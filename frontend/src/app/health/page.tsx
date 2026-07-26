import type { Metadata } from 'next';
import Link from 'next/link';
import { HealthPanel } from '../../features/api-health';
import { getApiHealth, type ApiHealth } from '../../shared/api';

export const metadata: Metadata = {
  title: 'API Health | Gaming Store',
  description: 'Current Gaming Store API connection status.',
};

async function loadApiHealth(): Promise<{
  health: ApiHealth | null;
  error: string | null;
}> {
  try {
    return {
      health: await getApiHealth(),
      error: null,
    };
  } catch {
    return {
      health: null,
      error: 'Backend is not reachable yet.',
    };
  }
}

export default async function HealthPage() {
  const { health, error } = await loadApiHealth();

  return (
    <main className="grid min-h-screen place-items-center bg-slate-950 p-4">
      <div className="grid w-full max-w-xl gap-5">
        <Link className="w-fit text-sm font-semibold text-cyan-300 hover:text-cyan-200" href="/">
          ← Back to store
        </Link>
        <HealthPanel health={health} error={error} />
      </div>
    </main>
  );
}
