using AutoMapper;
using Microsoft.EntityFrameworkCore;
using YallaShop.Application;
using YallaShop.Application.DTOs.Cart;
using YallaShop.Application.IRepositories;
using YallaShop.Application.IServices;
using YallaShop.Domain.Entites;
using YallaShop.Domain.Enums;
using YallaShop.Infrastructure.Persistence;

namespace YallaShop.Infrastructure.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepo;
        private readonly ICartItemRepository _cartItemRepo;
        private readonly IGenericRepository<Product> _productRepo;
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;

        public CartService(
            ICartRepository cartRepository,
            ICartItemRepository cartItemRepository,
            IGenericRepository<Product> productRepository,
            IMapper mapper,
            AppDbContext context)
        {
            _cartRepo = cartRepository;
            _cartItemRepo = cartItemRepository;
            _productRepo = productRepository;
            _mapper = mapper;
            _context = context;
        }

        public async Task<ResponseModel<CartDto>> GetCartAsync(GetCartDto dto)
        {
            try
            {
                var cart = await _cartRepo.GetCartWithItemsAsync(dto.UserId, dto.GuestSessionId);
                if (cart == null)
                {
                    return new ResponseModel<CartDto>
                    {
                        IsSuccess = true,
                        Message = "Cart is empty.",
                        Data = new CartDto
                        {
                            UserId = dto.UserId ?? string.Empty,
                            Items = Array.Empty<CartItemDto>(),
                            Subtotal = 0
                        }
                    };
                }

                return new ResponseModel<CartDto>
                {
                    IsSuccess = true,
                    Message = "Cart loaded.",
                    Data = _mapper.Map<CartDto>(cart)
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel<CartDto>
                {
                    IsSuccess = false,
                    Message = $"Error: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<ResponseModel<CartDto>> AddToCartAsync(AddToCartDto dto)
        {
            try
            {
                if (dto.Quantity < 1)
                {
                    return Fail("Quantity must be at least 1.");
                }

                var product = await _productRepo.GetAllAsync()
                    .FirstOrDefaultAsync(p => p.Id == dto.ProductId && !p.IsDeleted);

                if (product == null)
                {
                    return Fail("Product not found.");
                }

                if (product.Status != ProductStatus.Accepted)
                {
                    return Fail("Product is not available for purchase.");
                }

                var cart = await _cartRepo.GetCartWithItemsAsync(dto.UserId, dto.GuestSessionId);
                if (cart == null)
                {
                    cart = new Cart
                    {
                        UserId = dto.UserId,
                        GuestSessionId = dto.GuestSessionId,
                        CartItems = new List<CartItem>()
                    };
                    await _cartRepo.AddAsync(cart);
                    await _context.SaveChangesAsync();
                }

                var existingItem = await _cartItemRepo.GetByCartIdAndProductIdAsync(cart.Id, dto.ProductId);
                if (existingItem != null)
                {
                    var newQty = existingItem.Quantity + dto.Quantity;
                    if (newQty > product.StockQuantity)
                    {
                        return Fail("Not enough stock for this product.");
                    }

                    existingItem.Quantity = newQty;
                    _cartItemRepo.Update(existingItem);
                }
                else
                {
                    if (dto.Quantity > product.StockQuantity)
                    {
                        return Fail("Not enough stock for this product.");
                    }

                    var item = new CartItem
                    {
                        CartId = cart.Id,
                        ProductId = dto.ProductId,
                        Quantity = dto.Quantity
                    };
                    await _cartItemRepo.AddAsync(item);
                }

                await _context.SaveChangesAsync();

                var updated = await _cartRepo.GetCartWithItemsAsync(dto.UserId, dto.GuestSessionId);
                return new ResponseModel<CartDto>
                {
                    IsSuccess = true,
                    Message = "Item added to cart.",
                    Data = _mapper.Map<CartDto>(updated!)
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel<CartDto>
                {
                    IsSuccess = false,
                    Message = $"Error: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<ResponseModel<CartDto>> UpdateCartItemAsync(UpdateCartItemDto dto)
        {
            try
            {
                if (dto.Quantity < 1)
                {
                    return Fail("Quantity must be at least 1.");
                }

                var item = await _cartItemRepo.GetByIdWithDetailsAsync(dto.CartItemId);
                if (item == null)
                {
                    return Fail("Cart line not found.");
                }

                var canEdit = item.Cart != null && !item.Cart.IsDeleted &&
                              ((dto.UserId != null && item.Cart.UserId == dto.UserId) ||
                               (dto.GuestSessionId != null && item.Cart.GuestSessionId == dto.GuestSessionId));
                if (!canEdit)
                {
                    return Fail("You cannot modify this cart item.");
                }

                if (item.Product == null || item.Product.IsDeleted)
                {
                    return Fail("Product is no longer available.");
                }

                if (dto.Quantity > item.Product.StockQuantity)
                {
                    return Fail("Not enough stock for this product.");
                }

                item.Quantity = dto.Quantity;
                _cartItemRepo.Update(item);
                await _context.SaveChangesAsync();

                var updated = await _cartRepo.GetCartWithItemsAsync(dto.UserId, dto.GuestSessionId);
                return new ResponseModel<CartDto>
                {
                    IsSuccess = true,
                    Message = "Cart updated.",
                    Data = _mapper.Map<CartDto>(updated!)
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel<CartDto>
                {
                    IsSuccess = false,
                    Message = $"Error: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<ResponseModel<bool>> RemoveCartItemAsync(int cartItemId)
        {
            try
            {
                var item = await _cartItemRepo.GetByIdWithDetailsAsync(cartItemId);
                if (item == null)
                {
                    return new ResponseModel<bool> { IsSuccess = false, Message = "Cart line not found.", Data = false };
                }
                item.IsDeleted = true;
                _cartItemRepo.Update(item);
                await _context.SaveChangesAsync();

                return new ResponseModel<bool>
                {
                    IsSuccess = true,
                    Message = "Item removed.",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel<bool>
                {
                    IsSuccess = false,
                    Message = $"Error: {ex.Message}",
                    Data = false
                };
            }
        }

        public async Task<ResponseModel<bool>> ClearCartAsync(string? userId, string? guestSessionId)
        {
            try
            {
                await _cartRepo.ClearCartAsync(userId, guestSessionId);
                return new ResponseModel<bool>
                {
                    IsSuccess = true,
                    Message = "Cart cleared.",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel<bool>
                {
                    IsSuccess = false,
                    Message = $"Error: {ex.Message}",
                    Data = false
                };
            }
        }

        private static ResponseModel<CartDto> Fail(string message) =>
            new ResponseModel<CartDto>
            {
                IsSuccess = false,
                Message = message,
                Data = null
            };
    }
}
