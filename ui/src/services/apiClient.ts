import axios from 'axios';

const API_BASE_URL = '/api';

const axiosInstance = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json'
  }
});

interface CreateOrderRequest {
  pieType: string;
  customer: {
    name: string;
    email: string;
    phone?: string;
  };
  deliveryAddress: {
    street: string;
    city: string;
    state: string;
    zip: string;
  };
}

interface OrderResponse {
  orderId: string;
  status: string;
  estimatedDelivery?: string;
  createdAt: string;
}

interface OrderDetail {
  orderId: string;
  pieType: string;
  customer: {
    name: string;
    email: string;
    phone?: string;
  };
  deliveryAddress: {
    street: string;
    city: string;
    state: string;
    zip: string;
  };
  status: string;
  createdAt: string;
  updatedAt: string;
  estimatedDelivery?: string;
  history: Array<{
    state: string;
    timestamp: string;
  }>;
}

interface OrderSummary {
  orderId: string;
  customerName: string;
  pieType: string;
  status: string;
  createdAt: string;
}

interface Recipe {
  id: string;
  name: string;
  description: string;
  bakingTime: number;
  difficulty: string;
}

export const apiClient = {
  async createOrder(request: CreateOrderRequest): Promise<OrderResponse> {
    const response = await axiosInstance.post<OrderResponse>('/orders', request);
    return response.data;
  },

  async getOrder(orderId: string): Promise<OrderDetail> {
    const response = await axiosInstance.get<OrderDetail>(`/orders/${orderId}`);
    return response.data;
  },

  async getAllOrders(): Promise<OrderSummary[]> {
    const response = await axiosInstance.get<OrderSummary[]>('/orders');
    return response.data;
  },

  async getCatalog(): Promise<Recipe[]> {
    const response = await axiosInstance.get<Recipe[]>('/orders/catalog');
    return response.data;
  }
};
