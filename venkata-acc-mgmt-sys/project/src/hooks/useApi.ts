import { useState, useEffect } from 'react';
import { ApiResponse } from '../services/api';

// Generic hook for API calls with loading and error states
export function useApi<T>(
  apiCall: () => Promise<ApiResponse<T>>,
  dependencies: any[] = []
) {
  const [data, setData] = useState<T | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    let isMounted = true;

    const fetchData = async () => {
      try {
        setLoading(true);
        setError(null);
        const response = await apiCall();
        
        if (isMounted) {
          if (response.success) {
            setData(response.data);
          } else {
            setError(response.message || 'An error occurred');
          }
        }
      } catch (err) {
        if (isMounted) {
          setError(err instanceof Error ? err.message : 'An error occurred');
        }
      } finally {
        if (isMounted) {
          setLoading(false);
        }
      }
    };

    fetchData();

    return () => {
      isMounted = false;
    };
  }, dependencies);

  return { data, loading, error, refetch: () => fetchData() };
}

// Hook for paginated API calls
export function usePaginatedApi<T>(
  apiCall: (page: number, pageSize: number, ...args: any[]) => Promise<ApiResponse<{ data: T[]; totalCount: number; pageNumber: number; pageSize: number; totalPages: number }>>,
  initialPage = 1,
  initialPageSize = 10,
  dependencies: any[] = []
) {
  const [data, setData] = useState<T[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [page, setPage] = useState(initialPage);
  const [pageSize, setPageSize] = useState(initialPageSize);
  const [totalCount, setTotalCount] = useState(0);
  const [totalPages, setTotalPages] = useState(0);

  const fetchData = async (currentPage = page, currentPageSize = pageSize, ...args: any[]) => {
    try {
      setLoading(true);
      setError(null);
      const response = await apiCall(currentPage, currentPageSize, ...args);
      
      if (response.success) {
        setData(response.data.data);
        setTotalCount(response.data.totalCount);
        setTotalPages(response.data.totalPages);
        setPage(response.data.pageNumber);
        setPageSize(response.data.pageSize);
      } else {
        setError(response.message || 'An error occurred');
      }
    } catch (err) {
      setError(err instanceof Error ? err.message : 'An error occurred');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchData(page, pageSize, ...dependencies);
  }, [page, pageSize, ...dependencies]);

  return {
    data,
    loading,
    error,
    page,
    pageSize,
    totalCount,
    totalPages,
    setPage,
    setPageSize,
    refetch: fetchData,
  };
}

// Hook for mutations (create, update, delete)
export function useMutation<T, P = any>(
  mutationFn: (params: P) => Promise<ApiResponse<T>>
) {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [data, setData] = useState<T | null>(null);

  const mutate = async (params: P) => {
    try {
      setLoading(true);
      setError(null);
      const response = await mutationFn(params);
      
      if (response.success) {
        setData(response.data);
        return response.data;
      } else {
        setError(response.message || 'An error occurred');
        throw new Error(response.message || 'An error occurred');
      }
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'An error occurred';
      setError(errorMessage);
      throw err;
    } finally {
      setLoading(false);
    }
  };

  return {
    mutate,
    loading,
    error,
    data,
    reset: () => {
      setError(null);
      setData(null);
    },
  };
}