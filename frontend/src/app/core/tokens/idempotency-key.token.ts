import { HttpContext, HttpContextToken } from '@angular/common/http';

const createIdempotencyKey = (): string => {
  if (typeof crypto !== 'undefined' && typeof crypto.randomUUID === 'function') {
    return crypto.randomUUID();
  }

  return `${Date.now()}-${Math.random().toString(36).slice(2, 12)}`;
};

export const IDEMPOTENCY_ENABLED = new HttpContextToken<boolean>(() => false);
export const IDEMPOTENCY_KEY = new HttpContextToken<string>(() => '');

export const buildIdempotencyContext = (key?: string): HttpContext =>
  new HttpContext()
    .set(IDEMPOTENCY_ENABLED, true)
    .set(IDEMPOTENCY_KEY, key || createIdempotencyKey());