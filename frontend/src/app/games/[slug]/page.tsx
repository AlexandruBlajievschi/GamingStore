import type { Metadata } from 'next';
import Image from 'next/image';
import Link from 'next/link';
import { notFound } from 'next/navigation';
import { getGameBySlug } from '../../../shared/api';

const priceFormatter = new Intl.NumberFormat('en-US', {
  style: 'currency',
  currency: 'USD',
});

type GameDetailPageProps = {
  params: Promise<{ slug: string }>;
};

export async function generateMetadata({ params }: GameDetailPageProps): Promise<Metadata> {
  const { slug } = await params;
  const game = await getGameBySlug(slug);

  if (!game) {
    return {
      title: 'Game not found | Gaming Store',
    };
  }

  return {
    title: `${game.title} | Gaming Store`,
    description: game.description ?? `Buy ${game.title} from Gaming Store.`,
    openGraph: game.coverImageUrl
      ? {
          images: [{ url: game.coverImageUrl, alt: `${game.title} cover artwork` }],
        }
      : undefined,
  };
}

export default async function GameDetailPage({ params }: GameDetailPageProps) {
  const { slug } = await params;
  const game = await getGameBySlug(slug);

  if (!game) {
    notFound();
  }

  return (
    <main className="min-h-screen bg-slate-950 text-white">
      <div className="mx-auto grid w-[min(1120px,calc(100%-2rem))] gap-10 py-10 md:grid-cols-[minmax(280px,420px)_1fr] md:items-center md:py-20">
        <div className="relative aspect-[2/3] overflow-hidden rounded-3xl border border-white/10 bg-slate-800 shadow-2xl shadow-cyan-950/30">
          {game.coverImageUrl ? (
            <Image
              className="object-cover"
              src={game.coverImageUrl}
              alt={`${game.title} cover artwork`}
              fill
              priority
              sizes="(min-width: 768px) 420px, calc(100vw - 2rem)"
            />
          ) : (
            <div className="grid h-full place-items-center bg-gradient-to-br from-cyan-950 to-slate-950 text-sm font-bold uppercase tracking-[0.22em] text-cyan-200">
              Cover coming soon
            </div>
          )}
        </div>

        <section className="grid gap-6">
          <Link className="text-sm font-semibold text-cyan-300 hover:text-cyan-200" href="/">
            ← Back to catalog
          </Link>
          <div className="grid gap-3">
            <p className="text-sm font-bold uppercase tracking-[0.22em] text-cyan-300">
              {game.sellerName}
            </p>
            <h1 className="text-5xl font-bold leading-none md:text-7xl">{game.title}</h1>
          </div>
          <p className="max-w-2xl text-lg leading-8 text-slate-300">
            {game.description ?? 'More details are coming soon.'}
          </p>
          <div className="flex flex-wrap items-center gap-5 border-t border-white/10 pt-6">
            <span className="text-3xl font-bold">{priceFormatter.format(game.price)}</span>
            <span className="rounded-full bg-emerald-400/15 px-4 py-2 text-sm font-bold text-emerald-300">
              Available
            </span>
          </div>
        </section>
      </div>
    </main>
  );
}
