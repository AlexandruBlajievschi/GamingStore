import type { Metadata } from 'next';
import './styles.css';

export const metadata: Metadata = {
  title: 'Gaming Store',
  description: 'A game storefront for players, sellers, and the first catalog features.',
};

type RootLayoutProps = {
  children: React.ReactNode;
};

export default function RootLayout({ children }: RootLayoutProps) {
  return (
    <html lang="en">
      <body>{children}</body>
    </html>
  );
}
