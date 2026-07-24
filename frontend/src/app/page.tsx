import { HealthPanel } from '../features/api-health';
import { getApiHealth, type ApiHealth } from '../shared/api';

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

export default async function HomePage() {
  const { health, error } = await loadApiHealth();

  return (
    <main className="mx-auto grid min-h-screen w-[min(1040px,calc(100%-2rem))] gap-8 py-16">
      <section className="grid max-w-3xl gap-4 self-end">
        <p className="text-sm font-bold uppercase text-teal-800">Gaming Store</p>
        <h1 className="text-5xl font-bold leading-none text-zinc-950 md:text-7xl">
          Frontend and backend are ready to grow together.
        </h1>
        <p className="text-lg text-slate-600">
          A Next.js and React storefront for players, sellers, and the first game catalog features.
        </p>
      </section>

      <HealthPanel health={health} error={error} />
    </main>
  );
}
