'use client';

import Link from 'next/link';
import { useRouter } from 'next/navigation';
import { useState } from 'react';
import type { FormEvent } from 'react';
import { login, register } from '../api/auth';
import { logAuthError, logAuthEvent } from '../lib/authLog';

type AuthFormProps = {
  mode: 'login' | 'register';
  initialError?: string | null;
};

export function AuthForm({ mode, initialError = null }: AuthFormProps) {
  const router = useRouter();
  const [firstName, setFirstName] = useState('');
  const [lastName, setLastName] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [showPassword, setShowPassword] = useState(false);
  const [error, setError] = useState<string | null>(initialError);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const isRegistration = mode === 'register';

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setError(null);
    setIsSubmitting(true);

    try {
      const user = isRegistration
        ? await register({ firstName, lastName, email, password })
        : await login({ email, password });

      logAuthEvent(isRegistration ? 'Registration succeeded:' : 'Login succeeded:', user);
      router.push('/');
      router.refresh();
    } catch (requestError) {
      const message =
        requestError instanceof Error ? requestError.message : 'Authentication failed.';
      setError(message);
      logAuthError(isRegistration ? 'Registration failed:' : 'Login failed:', requestError);
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <main className="grid min-h-screen place-items-center bg-[radial-gradient(circle_at_top,_#164e63,_#020617_55%)] px-4 py-12 text-white">
      <section className="w-full max-w-md rounded-3xl border border-white/10 bg-slate-950/90 p-6 shadow-2xl shadow-slate-950/60 backdrop-blur md:p-8">
        <div className="grid gap-2">
          <Link
            className="w-fit text-sm font-semibold text-cyan-300 hover:text-cyan-200"
            href={isRegistration ? '/login' : '/'}
          >
            &larr; {isRegistration ? 'Back to log in' : 'Back to the store'}
          </Link>
          <h1 className="mt-4 text-3xl font-bold">
            {isRegistration ? 'Create your account' : 'Welcome back'}
          </h1>
          <p className="text-sm leading-6 text-slate-400">
            {isRegistration
              ? 'Register with an email address and a memorable passphrase.'
              : 'Log in to continue to your account.'}
          </p>
        </div>

        <a
          className="mt-8 flex h-12 items-center justify-center gap-3 rounded-xl border border-white/20 bg-white px-5 font-bold text-slate-900 transition hover:bg-slate-100 focus:outline-none focus:ring-4 focus:ring-white/20"
          href="/api/auth/google"
        >
          <svg className="size-5" viewBox="0 0 18 18" aria-hidden="true">
            <path
              fill="#4285f4"
              d="M17.64 9.205c0-.638-.057-1.252-.164-1.841H9v3.482h4.844a4.14 4.14 0 0 1-1.797 2.715v2.258h2.909c1.702-1.567 2.684-3.874 2.684-6.614Z"
            />
            <path
              fill="#34a853"
              d="M9 18c2.43 0 4.468-.806 5.956-2.181l-2.909-2.258c-.806.54-1.835.859-3.047.859-2.344 0-4.328-1.585-5.037-3.714H.956v2.332A9 9 0 0 0 9 18Z"
            />
            <path
              fill="#fbbc05"
              d="M3.963 10.706A5.41 5.41 0 0 1 3.682 9c0-.593.102-1.17.281-1.706V4.962H.956A9 9 0 0 0 0 9c0 1.45.347 2.824.956 4.038l3.007-2.332Z"
            />
            <path
              fill="#ea4335"
              d="M9 3.58c1.321 0 2.508.454 3.441 1.346l2.581-2.581C13.464.892 11.426 0 9 0A9 9 0 0 0 .956 4.962l3.007 2.332C4.672 5.165 6.656 3.58 9 3.58Z"
            />
          </svg>
          Continue with Google
        </a>

        <div className="my-5 flex items-center gap-3 text-xs font-semibold uppercase text-slate-500">
          <span className="h-px flex-1 bg-white/10" />
          or use email
          <span className="h-px flex-1 bg-white/10" />
        </div>

        <form className="grid gap-5" onSubmit={handleSubmit}>
          {isRegistration ? (
            <div className="grid gap-5 sm:grid-cols-2 sm:gap-6">
              <label className="grid min-w-0 gap-2 text-sm font-semibold">
                First name
                <input
                  className="h-12 w-full rounded-xl border border-white/15 bg-slate-900 px-4 text-white outline-none transition placeholder:text-slate-600 focus:border-cyan-300 focus:ring-4 focus:ring-cyan-300/15"
                  name="firstName"
                  autoComplete="given-name"
                  maxLength={100}
                  required
                  value={firstName}
                  onChange={(event) => setFirstName(event.target.value)}
                />
              </label>
              <label className="grid min-w-0 gap-2 text-sm font-semibold">
                Last name
                <input
                  className="h-12 w-full rounded-xl border border-white/15 bg-slate-900 px-4 text-white outline-none transition placeholder:text-slate-600 focus:border-cyan-300 focus:ring-4 focus:ring-cyan-300/15"
                  name="lastName"
                  autoComplete="family-name"
                  maxLength={100}
                  required
                  value={lastName}
                  onChange={(event) => setLastName(event.target.value)}
                />
              </label>
            </div>
          ) : null}

          <label className="grid gap-2 text-sm font-semibold">
            Email address
            <input
              className="h-12 rounded-xl border border-white/15 bg-slate-900 px-4 text-white outline-none transition placeholder:text-slate-600 focus:border-cyan-300 focus:ring-4 focus:ring-cyan-300/15"
              type="email"
              name="email"
              autoComplete="email"
              maxLength={320}
              required
              value={email}
              onChange={(event) => setEmail(event.target.value)}
            />
          </label>

          <label className="grid gap-2 text-sm font-semibold">
            Password
            <span className="relative">
              <input
                className="h-12 w-full rounded-xl border border-white/15 bg-slate-900 px-4 pr-16 text-white outline-none transition placeholder:text-slate-600 focus:border-cyan-300 focus:ring-4 focus:ring-cyan-300/15"
                type={showPassword ? 'text' : 'password'}
                name="password"
                autoComplete={isRegistration ? 'new-password' : 'current-password'}
                minLength={isRegistration ? 15 : undefined}
                maxLength={128}
                required
                value={password}
                onChange={(event) => setPassword(event.target.value)}
              />
              <button
                className="absolute right-3 top-1/2 -translate-y-1/2 text-xs font-bold text-cyan-300 hover:text-cyan-200"
                type="button"
                aria-pressed={showPassword}
                onClick={() => setShowPassword((current) => !current)}
              >
                {showPassword ? 'Hide' : 'Show'}
              </button>
            </span>
            {isRegistration ? (
              <span className="font-normal text-slate-400">
                Use at least 15 characters. A short sentence is easier to remember.
              </span>
            ) : null}
          </label>

          {error ? (
            <p
              className="rounded-xl border border-rose-400/30 bg-rose-400/10 p-3 text-sm text-rose-100"
              role="alert"
            >
              {error}
            </p>
          ) : null}

          <button
            className="h-12 rounded-xl bg-cyan-300 px-5 font-bold text-slate-950 transition hover:bg-cyan-200 disabled:cursor-wait disabled:opacity-60"
            type="submit"
            disabled={isSubmitting}
          >
            {isSubmitting
              ? isRegistration
                ? 'Creating account...'
                : 'Logging in...'
              : isRegistration
                ? 'Create account'
                : 'Log in'}
          </button>
        </form>

        <div className="pt-5">
          <p className="text-center text-sm text-slate-400">
            {isRegistration ? 'Already have an account?' : 'New to Gaming Store?'}{' '}
            <Link
              className="font-bold text-cyan-300 hover:text-cyan-200"
              href={isRegistration ? '/login' : '/register'}
            >
              {isRegistration ? 'Log in' : 'Create an account'}
            </Link>
          </p>
        </div>
      </section>
    </main>
  );
}
