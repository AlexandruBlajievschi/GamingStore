export type AuthenticatedUser = {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
};

export type LoginCredentials = {
  email: string;
  password: string;
};

export type RegistrationDetails = LoginCredentials & {
  firstName: string;
  lastName: string;
};

type AntiforgeryTokenResponse = {
  token: string;
};

type ProblemDetails = {
  title?: string;
  detail?: string;
  errors?: Record<string, string[]>;
};

async function getAntiforgeryToken(): Promise<string> {
  const response = await fetch('/api/auth/antiforgery-token', {
    cache: 'no-store',
    credentials: 'include',
  });

  if (!response.ok) {
    throw new Error('Could not start a secure authentication request.');
  }

  const result = (await response.json()) as AntiforgeryTokenResponse;

  return result.token;
}

async function getErrorMessage(response: Response): Promise<string> {
  const fallback = `Authentication request failed with status ${response.status}.`;

  try {
    const problem = (await response.json()) as ProblemDetails;
    const validationErrors = problem.errors ? Object.values(problem.errors).flat().join(' ') : null;

    return problem.detail ?? validationErrors ?? problem.title ?? fallback;
  } catch {
    return fallback;
  }
}

async function postAuth<TResponse>(path: string, body: object): Promise<TResponse> {
  const antiforgeryToken = await getAntiforgeryToken();
  const response = await fetch(path, {
    method: 'POST',
    credentials: 'include',
    headers: {
      'Content-Type': 'application/json',
      'X-CSRF-TOKEN': antiforgeryToken,
    },
    body: JSON.stringify(body),
  });

  if (!response.ok) {
    throw new Error(await getErrorMessage(response));
  }

  return response.json() as Promise<TResponse>;
}

export async function getCurrentUser(): Promise<AuthenticatedUser | null> {
  const response = await fetch('/api/auth/me', {
    cache: 'no-store',
    credentials: 'include',
  });

  if (response.status === 401) {
    return null;
  }

  if (!response.ok) {
    throw new Error(`Current-user request failed with status ${response.status}.`);
  }

  return response.json() as Promise<AuthenticatedUser>;
}

export function login(credentials: LoginCredentials): Promise<AuthenticatedUser> {
  return postAuth<AuthenticatedUser>('/api/auth/login', credentials);
}

export function register(details: RegistrationDetails): Promise<AuthenticatedUser> {
  return postAuth<AuthenticatedUser>('/api/auth/register', details);
}

export async function logout(): Promise<void> {
  const antiforgeryToken = await getAntiforgeryToken();
  const response = await fetch('/api/auth/logout', {
    method: 'POST',
    credentials: 'include',
    headers: {
      'Content-Type': 'application/json',
      'X-CSRF-TOKEN': antiforgeryToken,
    },
    body: '{}',
  });

  if (!response.ok) {
    throw new Error(await getErrorMessage(response));
  }
}
