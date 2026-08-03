export type AddressType = 'Shipping' | 'Billing';

export interface Address {
  id: string;
  customerId: string;
  type: AddressType;
  line1: string;
  line2: string | null;
  city: string;
  state: string;
  postalCode: string;
  country: string;
  isDefault: boolean;
}

export interface AddAddressRequest {
  type: AddressType;
  line1: string;
  line2?: string | null;
  city: string;
  state: string;
  postalCode: string;
  country: string;
  isDefault?: boolean;
}
