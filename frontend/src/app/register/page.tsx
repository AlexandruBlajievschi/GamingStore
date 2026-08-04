import type { Metadata } from 'next';
import { AuthForm } from '../../features/auth';

export const metadata: Metadata = {
  title: 'Create account | Gaming Store',
  robots: { index: false, follow: false },
};

export default function RegisterPage() {
  return <AuthForm mode="register" />;
}
