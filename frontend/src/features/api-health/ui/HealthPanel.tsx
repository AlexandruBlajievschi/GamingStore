import type { ApiHealth } from '../../../shared/api';

type HealthPanelProps = {
  health: ApiHealth | null;
  error: string | null;
};

export function HealthPanel({ health, error }: HealthPanelProps) {
  const state = error ? 'error' : health ? 'ok' : 'loading';

  return (
    <section className="health-panel" aria-live="polite">
      <h2>API Connection</h2>
      <span className="health-status" data-state={state}>
        {state === 'loading' ? 'Checking' : state.toUpperCase()}
      </span>
      <p className="health-message">
        {error ?? health?.message ?? 'Waiting for the backend response.'}
      </p>
    </section>
  );
}
