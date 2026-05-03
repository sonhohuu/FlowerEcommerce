import { useState, useEffect, useCallback } from "react";
import { productApi } from "../api/productApi";

export function useProducts(initialParams = {}) {
  const [data, setData]       = useState({ items: [], total: 0, page: 1, totalPages: 1 });
  const [loading, setLoading] = useState(true);
  const [error, setError]     = useState(null);
  const [params, setParams]   = useState({ page: 1, size: 10, ...initialParams });

  const fetch = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const res = await productApi.getAll(params);
      setData(res);
    } catch (e) {
      setError(e.message);
    } finally {
      setLoading(false);
    }
  }, [params]);

  useEffect(() => { fetch(); }, [fetch]);

  return { ...data, loading, error, params, setParams, refetch: fetch };
}