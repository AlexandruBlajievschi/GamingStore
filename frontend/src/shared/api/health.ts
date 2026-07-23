import type { ApiHealth } from './types';

export async function getApiHealth(): Promise<ApiHealth> {
  const response = await fetch('/api/health');

  if (!response.ok) {
    throw new Error(`Health check failed with status ${response.status}`);
  }

  return response.json() as Promise<ApiHealth>;
}
