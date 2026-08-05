import type { Metadata } from 'next';
import { AuthForm } from '../../features/auth';

export const metadata: Metadata = {
  title: 'Log in | Gaming Store',
  robots: { index: false, follow: false },
};

const authenticationErrors: Record<string, string> = {
  'google-account-exists':
    'An account already uses this email. Log in with its password, then connect Google from the account menu.',
  'google-denied': 'Google sign-in was cancelled.',
  'google-failed': 'Google could not sign you in. Please try again.',
  'google-not-configured': 'Google sign-in has not been configured for this environment.',
  'locked-out': 'This account is temporarily locked. Please try again later.',
};

type LoginPageProps = {
  searchParams: Promise<{ authError?: string }>;
};

export default async function LoginPage({ searchParams }: LoginPageProps) {
  const { authError } = await searchParams;
  const initialError = authError ? authenticationErrors[authError] : null;

  return <AuthForm mode="login" initialError={initialError} />;
}
