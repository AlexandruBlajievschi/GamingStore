'use client';

import Image from 'next/image';
import Link from 'next/link';
import { useRouter } from 'next/navigation';
import { useId, useMemo, useRef, useState } from 'react';
import type { FocusEvent, KeyboardEvent } from 'react';
import type { Game } from '../../../shared/api';

const priceFormatter = new Intl.NumberFormat('en-US', {
  style: 'currency',
  currency: 'USD',
});

const maximumResults = 6;

type GameSearchProps = {
  games: Game[];
};

export function GameSearch({ games }: GameSearchProps) {
  const router = useRouter();
  const [query, setQuery] = useState('');
  const [isOpen, setIsOpen] = useState(false);
  const [activeIndex, setActiveIndex] = useState(-1);
  const searchId = useId();
  const containerRef = useRef<HTMLDivElement>(null);
  const listboxId = `${searchId}-results`;

  const results = useMemo(() => {
    const normalizedQuery = query.trim().toLocaleLowerCase();

    if (!normalizedQuery) {
      return [];
    }

    return games
      .filter((game) => game.title.toLocaleLowerCase().includes(normalizedQuery))
      .sort((firstGame, secondGame) => {
        const firstStartsWithQuery = firstGame.title
          .toLocaleLowerCase()
          .startsWith(normalizedQuery);
        const secondStartsWithQuery = secondGame.title
          .toLocaleLowerCase()
          .startsWith(normalizedQuery);

        if (firstStartsWithQuery !== secondStartsWithQuery) {
          return firstStartsWithQuery ? -1 : 1;
        }

        return firstGame.title.localeCompare(secondGame.title);
      })
      .slice(0, maximumResults);
  }, [games, query]);

  const showPanel = isOpen && query.trim().length > 0;

  function handleKeyDown(event: KeyboardEvent<HTMLInputElement>) {
    if (event.key === 'Escape') {
      setIsOpen(false);
      setActiveIndex(-1);
      return;
    }

    if (results.length === 0) {
      return;
    }

    if (event.key === 'ArrowDown') {
      event.preventDefault();
      setIsOpen(true);
      setActiveIndex((currentIndex) => (currentIndex < results.length - 1 ? currentIndex + 1 : 0));
    }

    if (event.key === 'ArrowUp') {
      event.preventDefault();
      setIsOpen(true);
      setActiveIndex((currentIndex) => (currentIndex > 0 ? currentIndex - 1 : results.length - 1));
    }

    if (event.key === 'Enter' && activeIndex >= 0) {
      event.preventDefault();
      router.push(`/games/${results[activeIndex].slug}`);
    }
  }

  function handleBlur(event: FocusEvent<HTMLDivElement>) {
    if (!containerRef.current?.contains(event.relatedTarget)) {
      setIsOpen(false);
      setActiveIndex(-1);
    }
  }

  return (
    <div ref={containerRef} className="w-full" onBlur={handleBlur}>
      <label className="sr-only" htmlFor={searchId}>
        Search games
      </label>
      <div className="relative mx-auto w-full max-w-2xl">
        <svg
          className="pointer-events-none absolute left-4 top-1/2 size-5 -translate-y-1/2 text-slate-400"
          aria-hidden="true"
          viewBox="0 0 24 24"
          fill="none"
          stroke="currentColor"
          strokeWidth="2"
        >
          <circle cx="11" cy="11" r="7" />
          <path d="m20 20-4-4" />
        </svg>
        <input
          id={searchId}
          className="h-14 w-full rounded-2xl border border-white/15 bg-slate-900/95 pl-12 pr-4 text-base text-white shadow-xl shadow-slate-950/30 outline-none transition placeholder:text-slate-500 hover:border-white/25 focus:border-cyan-300 focus:ring-4 focus:ring-cyan-300/15"
          type="search"
          role="combobox"
          placeholder="Search by game title"
          value={query}
          autoComplete="off"
          spellCheck="false"
          aria-autocomplete="list"
          aria-controls={listboxId}
          aria-expanded={showPanel}
          aria-activedescendant={
            activeIndex >= 0 ? `${listboxId}-option-${activeIndex}` : undefined
          }
          onChange={(event) => {
            setQuery(event.target.value);
            setIsOpen(true);
            setActiveIndex(-1);
          }}
          onFocus={() => setIsOpen(true)}
          onKeyDown={handleKeyDown}
        />

        {showPanel ? (
          <div className="absolute z-20 mt-2 w-full overflow-hidden rounded-2xl border border-white/10 bg-slate-900 shadow-2xl shadow-slate-950/60">
            {results.length > 0 ? (
              <ul id={listboxId} role="listbox" aria-label="Game suggestions">
                {results.map((game, index) => (
                  <li
                    key={game.id}
                    role="none"
                    className="border-b border-white/10 last:border-b-0"
                  >
                    <Link
                      id={`${listboxId}-option-${index}`}
                      role="option"
                      aria-selected={activeIndex === index}
                      className={`grid grid-cols-[3rem_1fr_auto] items-center gap-3 p-3 transition ${
                        activeIndex === index
                          ? 'bg-cyan-300/15'
                          : 'hover:bg-white/[0.06] focus:bg-white/[0.06]'
                      }`}
                      href={`/games/${game.slug}`}
                      onMouseEnter={() => setActiveIndex(index)}
                      onFocus={() => setActiveIndex(index)}
                    >
                      <span className="relative block aspect-[2/3] overflow-hidden rounded-lg bg-slate-800">
                        {game.coverImageUrl ? (
                          <Image
                            className="object-cover"
                            src={game.coverImageUrl}
                            alt=""
                            fill
                            sizes="48px"
                          />
                        ) : (
                          <span className="grid h-full place-items-center text-xs font-bold text-cyan-200">
                            {game.title.slice(0, 1)}
                          </span>
                        )}
                      </span>
                      <span className="min-w-0 truncate font-semibold text-white">
                        {game.title}
                      </span>
                      <span className="font-bold text-cyan-200">
                        {priceFormatter.format(game.price)}
                      </span>
                    </Link>
                  </li>
                ))}
              </ul>
            ) : (
              <p className="p-4 text-sm text-slate-300" role="status">
                No games found.
              </p>
            )}
          </div>
        ) : null}
      </div>
    </div>
  );
}
