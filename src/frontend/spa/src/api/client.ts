const BASE = import.meta.env.VITE_API_BASE_URL || '/api';
const TOKEN_KEY = 'jwt';

function auth(token?: string | null | undefined): HeadersInit {
  const t = token ?? localStorage.getItem(TOKEN_KEY);
  const h: Record<string, string> = { 'Content-Type': 'application/json' };
  if (t) h['Authorization'] = `Bearer ${t}`;
  return h;
}

export async function get<T>(url: string, token?: string | null): Promise<T> {
  const r = await fetch(`${BASE}${url}`, { headers: auth(token) });
  if (!r.ok) throw new Error(`${r.status} ${r.statusText}`);
  return r.json() as Promise<T>;
}

export async function post<T>(url: string, body: unknown, token?: string | null, extraHeaders?: Record<string, string>): Promise<T> {
  const h = auth(token) as Record<string, string>;
  if (extraHeaders) {
    for (const [key, val] of Object.entries(extraHeaders)) {
      h[key] = val;
    }
  }
  const r = await fetch(`${BASE}${url}`, { method: 'POST', headers: h, body: JSON.stringify(body) });
  if (!r.ok) throw new Error(`${r.status} ${r.statusText}`);
  return r.json() as Promise<T>;
}

export async function put<T>(url: string, body: unknown, token?: string | null): Promise<T> {
  const r = await fetch(`${BASE}${url}`, { method: 'PUT', headers: auth(token), body: JSON.stringify(body) });
  if (!r.ok) throw new Error(`${r.status} ${r.statusText}`);
  return r.json() as Promise<T>;
}

export async function del(url: string, token?: string | null): Promise<void> {
  const r = await fetch(`${BASE}${url}`, { method: 'DELETE', headers: auth(token) });
  if (!r.ok) throw new Error(`${r.status} ${r.statusText}`);
}