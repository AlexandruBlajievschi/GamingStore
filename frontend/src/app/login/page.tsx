import type { Metadata } from 'next';
import { AuthForm } from '../../features/auth';

export const metadata: Metadata = {
  title: 'Log in | Gaming Store',
  robots: { index: false, follow: false },
};

export default function LoginPage() {
  return <AuthForm mode="login" />;
}
