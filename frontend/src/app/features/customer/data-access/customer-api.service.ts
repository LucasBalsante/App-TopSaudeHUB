import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { ApiResponse } from '@core/models/api-response.model';
import { BaseApiService } from '@core/services/base-api.service';

export interface CustomerApiModel {
  id: string;
  name: string;
  email: string;
  document: string;
}

export interface UpsertCustomerPayload {
  name: string;
  email: string;
  document: string;
}

@Injectable({ providedIn: 'root' })
export class CustomerApiService extends BaseApiService {
  private readonly resourcePath = 'api/customers';

  list(): Observable<CustomerApiModel[]> {
    return this.http
      .get<ApiResponse<CustomerApiModel[]>>(this.buildPath(this.resourcePath))
      .pipe(map((response) => response.data));
  }

  getById(customerId: string): Observable<CustomerApiModel> {
    return this.http
      .get<ApiResponse<CustomerApiModel>>(this.buildPath(`${this.resourcePath}/${customerId}`))
      .pipe(map((response) => response.data));
  }

  create(payload: UpsertCustomerPayload, idempotencyKey?: string): Observable<ApiResponse<CustomerApiModel>> {
    return this.http.post<ApiResponse<CustomerApiModel>>(this.buildPath(this.resourcePath), payload, {
      context: this.buildMutationContext(idempotencyKey)
    });
  }

  update(customerId: string, payload: UpsertCustomerPayload, idempotencyKey?: string): Observable<ApiResponse<CustomerApiModel>> {
    return this.http.put<ApiResponse<CustomerApiModel>>(this.buildPath(`${this.resourcePath}/${customerId}`), payload, {
      context: this.buildMutationContext(idempotencyKey)
    });
  }

  remove(customerId: string, idempotencyKey?: string): Observable<void> {
    return this.http
      .delete<ApiResponse<null>>(this.buildPath(`${this.resourcePath}/${customerId}`), {
        context: this.buildMutationContext(idempotencyKey)
      })
      .pipe(map(() => void 0));
  }
}