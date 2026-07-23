import { useEffect, useState } from 'react';
import { HealthPanel } from '../features/api-health';
import { getApiHealth, type ApiHealth } from '../shared/api';

export function App() {
  const [health, setHealth] = useState<ApiHealth | null>(null);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    getApiHealth()
      .then(setHealth)
      .catch(() => setError('Backend is not reachable yet.'));
  }, []);

  return (
    <main className="app-shell">
      <section className="intro">
        <p className="eyebrow">Gaming Store</p>
        <h1>Frontend and backend are ready to grow together.</h1>
        <p>
          A React TypeScript storefront for players, sellers, and the first
          game catalog features.
        </p>
      </section>

      <HealthPanel health={health} error={error} />
    </main>
  );
}
