import type { ApiHealth } from './types';

export async function getApiHealth(): Promise<ApiHealth> {
  const apiBaseUrl =
    typeof window === 'undefined' ? (process.env.API_BASE_URL ?? 'http://localhost:5215') : '';

  const response = await fetch(`${apiBaseUrl}/api/health`, {
    cache: 'no-store',
  });

  if (!response.ok) {
    throw new Error(`Health check failed with status ${response.status}`);
  }

  return response.json() as Promise<ApiHealth>;
}
