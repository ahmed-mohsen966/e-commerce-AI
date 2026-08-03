export interface Category {
  id: string;
  name: string;
  description: string | null;
  parentCategoryId: string | null;
}

export interface CreateCategoryRequest {
  name: string;
  description?: string | null;
  parentCategoryId?: string | null;
}
