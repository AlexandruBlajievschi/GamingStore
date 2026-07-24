import type { ApiHealth } from '../../../shared/api';

type HealthPanelProps = {
  health: ApiHealth | null;
  error: string | null;
};

export function HealthPanel({ health, error }: HealthPanelProps) {
  const state = error ? 'error' : health ? 'ok' : 'loading';
  const statusClassName =
    state === 'error'
      ? 'bg-red-100 text-red-900'
      : state === 'loading'
        ? 'bg-slate-200 text-slate-600'
        : 'bg-emerald-100 text-emerald-900';

  return (
    <section
      className="grid max-w-xl gap-3 rounded-lg border border-slate-200 bg-white p-5 shadow-xl shadow-slate-900/10"
      aria-live="polite"
    >
      <h2 className="text-xl font-bold text-zinc-950">API Connection</h2>
      <span
        className={`inline-flex min-h-8 w-fit items-center rounded-full px-3 py-1 font-bold ${statusClassName}`}
      >
        {state === 'loading' ? 'Checking' : state.toUpperCase()}
      </span>
      <p className="text-slate-600">
        {error ?? health?.message ?? 'Waiting for the backend response.'}
      </p>
    </section>
  );
}
