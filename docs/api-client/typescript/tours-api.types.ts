/**
 * TypeScript Type Definitions for Tours and Activities API
 * Auto-generated from API specification
 */

// =============================================
// Core API Types
// =============================================

export interface ApiResponse<T> {
  success: boolean;
  data: T;
  message?: string;
  timestamp: string;
  requestId: string;
}

export interface ApiError {
  success: false;
  error: {
    code: string;
    message: string;
    details?: ErrorDetail[];
  };
  timestamp: string;
  requestId: string;
}

export interface ErrorDetail {
  field: string;
  message: string;
}

// =============================================
// Activity Types
// =============================================

export interface Activity {
  id: string;
  name: string;
  description: string;
  shortDescription?: string;
  duration: string;
  category: Category;
  destination: Destination;
  supplier: Supplier;
  price: Price;
  rating: number;
  reviewCount: number;
  images: string[];
  availability: AvailabilityStatus;
  features: string[];
  inclusions: string[];
  exclusions: string[];
  cancellationPolicy: CancellationPolicy;
}

export interface Category {
  id: number;
  name: string;
  slug: string;
  description?: string;
  iconUrl?: string;
}

export interface Destination {
  id: number;
  name: string;
  countryCode: string;
  cityCode?: string;
  latitude?: number;
  longitude?: number;
  timeZone?: string;
}

export interface Supplier {
  id: number;
  name: string;
  code: string;
  rating?: number;
}

export interface Price {
  amount: number;
  currency: string;
  originalAmount?: number;
  discount?: Discount;
}

export interface Discount {
  type: 'percentage' | 'fixed';
  value: number;
  code?: string;
}

export enum AvailabilityStatus {
  Available = 'Available',
  LimitedAvailability = 'LimitedAvailability',
  SoldOut = 'SoldOut',
  OnRequest = 'OnRequest'
}

export interface CancellationPolicy {
  cancellable: boolean;
  deadline?: string;
  refundPercentage: number;
  terms?: string;
}

// =============================================
// Search & Filter Types
// =============================================

export interface SearchRequest {
  keyword?: string;
  destination?: string;
  destinationId?: number;
  categoryId?: number;
  startDate?: string;
  endDate?: string;
  adults?: number;
  children?: number;
  minPrice?: number;
  maxPrice?: number;
  rating?: number;
  page?: number;
  pageSize?: number;
  sortBy?: SortOption;
}

export enum SortOption {
  Relevance = 'relevance',
  PriceLowToHigh = 'price_asc',
  PriceHighToLow = 'price_desc',
  Rating = 'rating',
  Popularity = 'popularity',
  Newest = 'newest'
}

export interface SearchResponse {
  results: Activity[];
  totalCount: number;
  page: number;
  pageSize: number;
  filters: FilterOptions;
}

export interface FilterOptions {
  categories: Category[];
  priceRange: PriceRange;
  destinations: Destination[];
  ratings: number[];
}

export interface PriceRange {
  min: number;
  max: number;
  currency: string;
}

// =============================================
// Booking Types
// =============================================

export interface BookingRequest {
  activityId: string;
  date: string;
  timeSlot?: string;
  participants: Participant[];
  customer: Customer;
  payment: PaymentInfo;
  specialRequests?: string;
  promoCode?: string;
}

export interface Participant {
  type: ParticipantType;
  firstName: string;
  lastName: string;
  age?: number;
  dateOfBirth?: string;
}

export enum ParticipantType {
  Adult = 'Adult',
  Child = 'Child',
  Infant = 'Infant',
  Senior = 'Senior',
  Student = 'Student'
}

export interface Customer {
  email: string;
  phone: string;
  firstName: string;
  lastName: string;
  country?: string;
  address?: Address;
}

export interface Address {
  street?: string;
  city?: string;
  state?: string;
  postalCode?: string;
  country: string;
}

export interface PaymentInfo {
  method: PaymentMethod;
  amount: number;
  currency: string;
  cardToken?: string;
}

export enum PaymentMethod {
  CreditCard = 'CreditCard',
  DebitCard = 'DebitCard',
  PayPal = 'PayPal',
  BankTransfer = 'BankTransfer'
}

export interface BookingResponse {
  bookingId: string;
  confirmationNumber: string;
  status: BookingStatus;
  totalAmount: number;
  currency: string;
  voucher: Voucher;
  cancellationPolicy: CancellationPolicy;
  customer: Customer;
  activity: Activity;
  bookingDate: string;
}

export enum BookingStatus {
  Pending = 'Pending',
  Confirmed = 'Confirmed',
  Cancelled = 'Cancelled',
  Completed = 'Completed',
  Failed = 'Failed'
}

export interface Voucher {
  url: string;
  qrCode: string;
  expiryDate?: string;
}

// =============================================
// Availability Types
// =============================================

export interface AvailabilityRequest {
  activityId: string;
  date: string;
  adults: number;
  children?: number;
}

export interface AvailabilityResponse {
  activityId: string;
  date: string;
  available: boolean;
  timeSlots: TimeSlot[];
  restrictions: Restrictions;
}

export interface TimeSlot {
  time: string;
  available: boolean;
  price: number;
  spotsRemaining: number;
}

export interface Restrictions {
  minAge?: number;
  maxAge?: number;
  minGroupSize?: number;
  maxGroupSize?: number;
  wheelchairAccessible: boolean;
  requirements?: string[];
}

// =============================================
// Authentication Types
// =============================================

export interface LoginRequest {
  username: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  expires: string;
  user: User;
}

export interface User {
  id: number;
  username: string;
  email: string;
  roles: string[];
}

