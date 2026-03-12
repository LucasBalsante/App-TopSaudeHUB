import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { ApiResponse } from '@core/models/api-response.model';
import { BaseApiService } from '@core/services/base-api.service';

export interface ProductApiModel {
  id: string;
  name: string;
  sku: string;
  price: number;
  stockQty: number;
  isActive: boolean;
}

export interface UpsertProductPayload {
  name: string;
  price: number;
  stockQty: number;
  isActive: boolean;
}

@Injectable({ providedIn: 'root' })
export class ProductApiService extends BaseApiService {
  private readonly resourcePath = 'api/products';

  list(): Observable<ProductApiModel[]> {
    return this.http
      .get<ApiResponse<ProductApiModel[]>>(this.buildPath(this.resourcePath))
      .pipe(map((response) => response.data));
  }

  getById(productId: string): Observable<ProductApiModel> {
    return this.http
      .get<ApiResponse<ProductApiModel>>(this.buildPath(`${this.resourcePath}/${productId}`))
      .pipe(map((response) => response.data));
  }

  create(payload: UpsertProductPayload, idempotencyKey?: string): Observable<ApiResponse<ProductApiModel>> {
    return this.http.post<ApiResponse<ProductApiModel>>(this.buildPath(this.resourcePath), payload, {
      context: this.buildMutationContext(idempotencyKey)
    });
  }

  update(productId: string, payload: UpsertProductPayload, idempotencyKey?: string): Observable<ApiResponse<ProductApiModel>> {
    return this.http.put<ApiResponse<ProductApiModel>>(this.buildPath(`${this.resourcePath}/${productId}`), payload, {
      context: this.buildMutationContext(idempotencyKey)
    });
  }

  remove(productId: string, idempotencyKey?: string): Observable<void> {
    return this.http.delete<ApiResponse<null>>(this.buildPath(`${this.resourcePath}/${productId}`), {
      context: this.buildMutationContext(idempotencyKey)
    }).pipe(map(() => void 0));
  }
}