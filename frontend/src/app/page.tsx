import { AuthNotification } from '../features/auth';
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

const authenticationNotices: Record<string, { message: string; isError: boolean }> = {
  'google-signed-in': { message: 'You are signed in with Google.', isError: false },
  'google-linked': { message: 'Google is now connected to your account.', isError: false },
  'google-in-use': {
    message: 'That Google account is already connected to another Gaming Store account.',
    isError: true,
  },
  'google-link-failed': {
    message: 'Google could not be connected. Please try again.',
    isError: true,
  },
  'google-link-denied': {
    message: 'Google connection was cancelled.',
    isError: true,
  },
  'google-not-configured': {
    message: 'Google sign-in has not been configured for this environment.',
    isError: true,
  },
};

type HomePageProps = {
  searchParams: Promise<{ authError?: string; authStatus?: string }>;
};

export default async function HomePage({ searchParams }: HomePageProps) {
  const { games, error: gamesError } = await loadGames();
  const { authError, authStatus } = await searchParams;
  const authenticationNotice = authenticationNotices[authError ?? authStatus ?? ''];
  const featuredGames = games.slice(0, 3);

  return (
    <main className="min-h-screen bg-slate-950 text-white">
      <h1 className="sr-only">Search the Gaming Store</h1>
      <StoreHeader games={games} />

      {authenticationNotice ? <AuthNotification {...authenticationNotice} /> : null}

      <section
        className="mx-auto grid w-[min(1180px,calc(100%-2rem))] gap-8 py-16"
        aria-labelledby="catalog-heading"
      >
        <div className="grid gap-2">
          <p className="text-sm font-bold uppercase tracking-[0.2em] text-cyan-300">
            Storefront test
          </p>
          <h2 id="catalog-heading" className="text-3xl font-bold md:text-4xl">
            Featured games
          </h2>
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
