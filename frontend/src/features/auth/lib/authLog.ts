import type { AuthenticatedUser } from '../api/auth';

export function logCurrentUser(user: AuthenticatedUser | null) {
  if (process.env.NODE_ENV !== 'development') {
    return;
  }

  console.info('[Gaming Store auth] Current user:', user ?? 'not authenticated');
}

export function logAuthEvent(message: string, user?: AuthenticatedUser) {
  if (process.env.NODE_ENV !== 'development') {
    return;
  }

  console.info(`[Gaming Store auth] ${message}`, user ?? '');
}

export function logAuthError(message: string, error: unknown) {
  if (process.env.NODE_ENV !== 'development') {
    return;
  }

  console.error(`[Gaming Store auth] ${message}`, error);
}
