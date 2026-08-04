import { GameCard } from '../features/games';
import { getGames, type Game } from '../shared/api';
import { StoreHeader } from '../widgets/store-header';

async function loadGames(): Promise<{
  games: Game[];
  error: string | null;
}> {
  try {
    return {
      games: await getGames(),
      error: null,
    };
  } catch {
    return {
      games: [],
      error: 'The game catalog is unavailable until the backend is running.',
    };
  }
}

export default async function HomePage() {
  const { games, error: gamesError } = await loadGames();
  const featuredGames = games.slice(0, 3);

  return (
    <main className="min-h-screen bg-slate-950 text-white">
      <h1 className="sr-only">Search the Gaming Store</h1>
      <StoreHeader games={games} />

      <section
        className="mx-auto grid w-[min(1180px,calc(100%-2rem))] gap-8 py-16"
        aria-labelledby="catalog-heading"
      >
        <div className="flex flex-wrap items-end justify-between gap-4">
          <div className="grid gap-2">
            <p className="text-sm font-bold uppercase tracking-[0.2em] text-cyan-300">
              Storefront test
            </p>
            <h2 id="catalog-heading" className="text-3xl font-bold md:text-4xl">
              Featured games
            </h2>
          </div>
          <p className="max-w-md text-sm text-slate-400">
            Product URLs and cover paths come directly from the ASP.NET Core API.
          </p>
        </div>

        {gamesError ? (
          <p className="rounded-2xl border border-amber-400/30 bg-amber-400/10 p-5 text-amber-100">
            {gamesError}
          </p>
        ) : (
          <div className="grid gap-6 sm:grid-cols-2 lg:grid-cols-3">
            {featuredGames.map((game) => (
              <GameCard key={game.id} game={game} />
            ))}
          </div>
        )}
      </section>
    </main>
  );
}
