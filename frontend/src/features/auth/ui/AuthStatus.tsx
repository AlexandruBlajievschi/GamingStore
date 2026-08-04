'use client';

import Link from 'next/link';
import { useRouter } from 'next/navigation';
import { useEffect, useState } from 'react';
import { getCurrentUser, logout } from '../api/auth';
import type { AuthenticatedUser } from '../api/auth';
import { logAuthError, logAuthEvent, logCurrentUser } from '../lib/authLog';

type AccountIconProps = {
  isAuthenticated: boolean;
};

function AccountIcon({ isAuthenticated }: AccountIconProps) {
  return (
    <span className="relative" aria-hidden="true">
      <svg
        className="size-6"
        viewBox="0 0 24 24"
        fill={isAuthenticated ? 'currentColor' : 'none'}
        stroke="currentColor"
        strokeWidth="1.8"
      >
        <circle cx="12" cy="8" r="4" />
        <path d="M4.5 20a7.5 7.5 0 0 1 15 0" />
      </svg>
      <span
        className={`absolute -bottom-2 -right-2 grid size-4 place-items-center rounded-full ring-2 ring-slate-950 ${
          isAuthenticated ? 'bg-emerald-400 text-emerald-950' : 'bg-slate-500'
        }`}
      >
        {isAuthenticated ? (
          <svg
            className="size-2.5"
            viewBox="0 0 12 12"
            fill="none"
            stroke="currentColor"
            strokeWidth="2"
          >
            <path d="m2.5 6 2 2 5-5" />
          </svg>
        ) : null}
      </span>
    </span>
  );
}

export function AuthStatus() {
  const router = useRouter();
  const [user, setUser] = useState<AuthenticatedUser | null>();
  const [isSigningOut, setIsSigningOut] = useState(false);
  const [logoutError, setLogoutError] = useState<string | null>(null);

  useEffect(() => {
    let isActive = true;

    getCurrentUser()
      .then((currentUser) => {
        if (!isActive) {
          return;
        }

        setUser(currentUser);
        logCurrentUser(currentUser);
      })
      .catch((error: unknown) => {
        if (!isActive) {
          return;
        }

        setUser(null);
        logAuthError('Could not load the current user:', error);
      });

    return () => {
      isActive = false;
    };
  }, []);

  async function handleLogout() {
    setLogoutError(null);
    setIsSigningOut(true);

    try {
      await logout();
      logAuthEvent('Logout succeeded.');
      setUser(null);
      router.refresh();
    } catch (error) {
      setLogoutError('Could not log out. Please try again.');
      logAuthError('Logout failed:', error);
    } finally {
      setIsSigningOut(false);
    }
  }

  if (user === undefined) {
    return (
      <span
        className="grid size-14 animate-pulse place-items-center rounded-2xl border border-white/10 bg-slate-900/70 text-slate-500"
        role="status"
        aria-label="Checking account status"
      >
        <AccountIcon isAuthenticated={false} />
      </span>
    );
  }

  if (!user) {
    return (
      <Link
        className="grid size-14 place-items-center rounded-2xl border border-white/15 bg-slate-900 text-slate-300 transition hover:border-cyan-300 hover:text-cyan-200 focus:outline-none focus:ring-4 focus:ring-cyan-300/15"
        href="/login"
        aria-label="Log in to your account"
        title="Not signed in — log in"
      >
        <AccountIcon isAuthenticated={false} />
      </Link>
    );
  }

  return (
    <details className="group relative">
      <summary
        className="grid size-14 cursor-pointer list-none place-items-center rounded-2xl border border-cyan-300/40 bg-cyan-300/15 text-cyan-100 transition hover:bg-cyan-300/25 focus:outline-none focus:ring-4 focus:ring-cyan-300/15"
        aria-label={`Open account menu for ${user.firstName} ${user.lastName}`}
        title={`Signed in as ${user.firstName} ${user.lastName}`}
      >
        <AccountIcon isAuthenticated />
      </summary>
      <div className="absolute right-0 z-30 mt-2 grid w-64 gap-3 rounded-2xl border border-white/10 bg-slate-900 p-4 shadow-2xl shadow-slate-950/70">
        <div className="min-w-0">
          <p className="truncate font-bold text-white">
            {user.firstName} {user.lastName}
          </p>
          <p className="truncate text-sm text-slate-400">{user.email}</p>
        </div>
        <button
          className="h-10 rounded-xl border border-white/15 px-3 text-sm font-bold text-white transition hover:border-rose-300 hover:text-rose-200 disabled:cursor-wait disabled:opacity-60"
          type="button"
          disabled={isSigningOut}
          onClick={handleLogout}
        >
          {isSigningOut ? 'Logging out...' : 'Log out'}
        </button>
        {logoutError ? (
          <p className="text-sm text-rose-200" role="alert">
            {logoutError}
          </p>
        ) : null}
      </div>
    </details>
  );
}
