import Image from 'next/image';
import Link from 'next/link';
import type { Game } from '../../../shared/api';

const priceFormatter = new Intl.NumberFormat('en-US', {
  style: 'currency',
  currency: 'USD',
});

type GameCardProps = {
  game: Game;
};

export function GameCard({ game }: GameCardProps) {
  return (
    <article className="group overflow-hidden rounded-3xl border border-white/10 bg-slate-900 shadow-2xl shadow-slate-950/20">
      <Link className="block" href={`/games/${game.slug}`}>
        <div className="relative aspect-[2/3] overflow-hidden bg-slate-800">
          {game.coverImageUrl ? (
            <Image
              className="object-cover transition duration-500 group-hover:scale-[1.03]"
              src={game.coverImageUrl}
              alt={`${game.title} cover artwork`}
              fill
              sizes="(min-width: 1024px) 28vw, (min-width: 640px) 45vw, 92vw"
            />
          ) : (
            <div className="grid h-full place-items-center bg-gradient-to-br from-cyan-950 to-slate-950 p-8 text-center text-sm font-bold uppercase tracking-[0.22em] text-cyan-200">
              Cover coming soon
            </div>
          )}
        </div>

        <div className="grid gap-3 p-5">
          <p className="text-xs font-bold uppercase tracking-[0.2em] text-cyan-300">
            {game.sellerName}
          </p>
          <div className="flex items-start justify-between gap-4">
            <h2 className="text-xl font-bold text-white">{game.title}</h2>
            <span className="shrink-0 font-bold text-white">
              {priceFormatter.format(game.price)}
            </span>
          </div>
          <span className="text-sm font-semibold text-slate-300 transition group-hover:text-cyan-200">
            View game →
          </span>
        </div>
      </Link>
    </article>
  );
}
