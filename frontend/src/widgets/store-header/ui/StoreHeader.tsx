import Link from 'next/link';
import { AuthStatus } from '../../../features/auth';
import { GameSearch } from '../../../features/games';
import type { Game } from '../../../shared/api';

type StoreHeaderProps = {
  games: Game[];
};

export function StoreHeader({ games }: StoreHeaderProps) {
  return (
    <header className="border-b border-white/10 bg-[radial-gradient(circle_at_top_left,_#164e63,_#020617_52%)]">
      <div className="mx-auto grid w-[min(1180px,calc(100%-2rem))] grid-cols-2 gap-3 py-8 md:grid-cols-[minmax(0,1fr)_auto_auto] md:items-start md:py-10">
        <div className="col-span-2 md:col-span-1">
          <GameSearch games={games} />
        </div>
        <AuthStatus />
        <Link
          className="relative grid size-14 place-items-center rounded-2xl bg-cyan-300 text-slate-950 transition hover:bg-cyan-200 focus:outline-none focus:ring-4 focus:ring-cyan-300/20"
          href="/cart"
          aria-label="Open cart, 0 items"
          title="Cart — 0 items"
        >
          <svg
            className="size-6"
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            strokeWidth="1.8"
            aria-hidden="true"
          >
            <path d="M3 4h2l2.2 10.2a2 2 0 0 0 2 1.6h7.9a2 2 0 0 0 1.9-1.4L21 8H6" />
            <circle cx="10" cy="20" r="1" fill="currentColor" />
            <circle cx="18" cy="20" r="1" fill="currentColor" />
          </svg>
          <span className="absolute -right-1.5 -top-1.5 grid size-5 place-items-center rounded-full bg-slate-950 text-[0.65rem] font-bold text-cyan-200 ring-2 ring-cyan-300">
            0
          </span>
        </Link>
      </div>
    </header>
  );
}
