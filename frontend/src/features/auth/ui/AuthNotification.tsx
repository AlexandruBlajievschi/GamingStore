'use client';

import { useEffect, useState } from 'react';

type AuthNotificationProps = {
  message: string;
  isError: boolean;
};

export function AuthNotification({ message, isError }: AuthNotificationProps) {
  const [isVisible, setIsVisible] = useState(true);

  useEffect(() => {
    const url = new URL(window.location.href);
    url.searchParams.delete('authError');
    url.searchParams.delete('authStatus');
    window.history.replaceState(
      window.history.state,
      '',
      `${url.pathname}${url.search}${url.hash}`,
    );

    const timeout = window.setTimeout(() => setIsVisible(false), 5_000);

    return () => window.clearTimeout(timeout);
  }, []);

  if (!isVisible) {
    return null;
  }

  return (
    <div
      className={`fixed left-1/2 top-6 z-50 flex w-[min(32rem,calc(100%-2rem))] -translate-x-1/2 items-center justify-center border px-12 py-4 text-center text-sm font-semibold shadow-2xl ${
        isError
          ? 'border-rose-400/40 bg-rose-950 text-rose-100 shadow-rose-950/40'
          : 'border-emerald-400/40 bg-emerald-950 text-emerald-100 shadow-emerald-950/40'
      }`}
      role={isError ? 'alert' : 'status'}
      aria-live={isError ? 'assertive' : 'polite'}
    >
      <span>{message}</span>
      <button
        className="absolute right-3 grid size-8 place-items-center text-xl font-normal text-current opacity-70 transition hover:opacity-100 focus:outline-none focus:ring-2 focus:ring-current"
        type="button"
        aria-label="Dismiss notification"
        onClick={() => setIsVisible(false)}
      >
        &times;
      </button>
    </div>
  );
}
