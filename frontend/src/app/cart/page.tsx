import type { Metadata } from 'next';
import Link from 'next/link';

export const metadata: Metadata = {
  title: 'Cart | Gaming Store',
  robots: { index: false, follow: false },
};

export default function CartPage() {
  return (
    <main className="grid min-h-screen place-items-center bg-slate-950 px-4 text-white">
      <section className="grid max-w-lg gap-4 text-center">
        <p className="text-sm font-bold uppercase tracking-[0.2em] text-cyan-300">Your cart</p>
        <h1 className="text-4xl font-bold">Your cart is empty</h1>
        <p className="text-slate-400">
          Cart persistence will be added with the shopping flow. The header action is ready in its
          final position.
        </p>
        <Link
          className="mx-auto mt-2 rounded-xl bg-cyan-300 px-5 py-3 font-bold text-slate-950 hover:bg-cyan-200"
          href="/"
        >
          Browse games
        </Link>
      </section>
    </main>
  );
}
