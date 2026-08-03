export type ProductSortBy = 'Name' | 'BasePrice' | 'CreatedAt';

export interface ProductListItem {
  id: string;
  name: string;
  categoryId: string;
  basePriceAmount: number;
  basePriceCurrency: string;
  isActive: boolean;
}

export interface ProductVariant {
  id: string;
  sku: string;
  size: string;
  color: string;
  priceAmount: number;
  priceCurrency: string;
  stockQuantity: number;
}

export interface Product {
  id: string;
  name: string;
  description: string;
  categoryId: string;
  basePriceAmount: number;
  basePriceCurrency: string;
  isActive: boolean;
  createdAt: string;
  variants: ProductVariant[];
}

export interface ProductListQuery {
  pageNumber?: number;
  pageSize?: number;
  categoryId?: string;
  minPrice?: number;
  maxPrice?: number;
  sortBy?: ProductSortBy;
  sortDescending?: boolean;
}

export interface CreateProductRequest {
  name: string;
  description: string;
  categoryId: string;
  basePriceAmount: number;
  basePriceCurrency: string;
}

export interface UpdateProductRequest extends CreateProductRequest {
  id: string;
  isActive: boolean;
}

export interface CreateProductVariantRequest {
  sku: string;
  size: string;
  color: string;
  priceAmount: number;
  priceCurrency: string;
  stockQuantity: number;
}
