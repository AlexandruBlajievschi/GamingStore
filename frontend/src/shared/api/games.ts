import type { Game } from './types';

const apiBaseUrl =
  typeof window === 'undefined' ? (process.env.API_BASE_URL ?? 'http://localhost:5215') : '';

export async function getGames(): Promise<Game[]> {
  const response = await fetch(`${apiBaseUrl}/api/games`, {
    cache: 'no-store',
  });

  if (!response.ok) {
    throw new Error(`Games request failed with status ${response.status}`);
  }

  return response.json() as Promise<Game[]>;
}

export async function getGameBySlug(slug: string): Promise<Game | null> {
  const response = await fetch(`${apiBaseUrl}/api/games/by-slug/${encodeURIComponent(slug)}`, {
    cache: 'no-store',
  });

  if (response.status === 404) {
    return null;
  }

  if (!response.ok) {
    throw new Error(`Game request failed with status ${response.status}`);
  }

  return response.json() as Promise<Game>;
}
