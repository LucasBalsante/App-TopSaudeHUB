import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { ApiResponse } from '@core/models/api-response.model';
import { BaseApiService } from '@core/services/base-api.service';

export interface OrderApiItemPayload {
  productId: string;
  Quantity: number;
}

export interface UpsertOrderPayload {
  customerId: string;
  Items: OrderApiItemPayload[];
  status?: number;
}

export interface OrderApiListProductModel {
  id: string;
  name: string;
  price: number;
}

export interface OrderApiListItemModel {
  product: OrderApiListProductModel;
  quantidade: number;
  lineTotal: number;
}

export interface OrderApiListCustomerModel {
  id: string;
  nome: string;
}

export interface OrderApiModel {
  id: string;
}

export interface OrderApiListModel {
  id: string;
  customer: OrderApiListCustomerModel;
  totalAmount: number;
  status: string;
  items: OrderApiListItemModel[];
}

export interface OrderApiListResponse {
  success: boolean;
  message: string | null;
  data: OrderApiListModel[];
}

export interface OrderMutationResponse<T> {
  success?: boolean;
  message?: string | null;
  cod_retorno?: number;
  mensagem?: string | null;
  data: T;
}

@Injectable({ providedIn: 'root' })
export class OrderApiService extends BaseApiService {
  private readonly resourcePath = 'api/orders';

  list(): Observable<OrderApiListModel[]> {
    return this.http
      .get<OrderApiListResponse>(this.buildPath(this.resourcePath))
      .pipe(map((response) => response.data));
  }

  create(payload: UpsertOrderPayload, idempotencyKey?: string): Observable<OrderMutationResponse<OrderApiModel>> {
    return this.http.post<OrderMutationResponse<OrderApiModel>>(this.buildPath(this.resourcePath), payload, {
      context: this.buildMutationContext(idempotencyKey)
    });
  }

  update(orderId: string, payload: UpsertOrderPayload, idempotencyKey?: string): Observable<OrderMutationResponse<OrderApiModel>> {
    return this.http.put<OrderMutationResponse<OrderApiModel>>(this.buildPath(`${this.resourcePath}/${orderId}`), payload, {
      context: this.buildMutationContext(idempotencyKey)
    });
  }
}