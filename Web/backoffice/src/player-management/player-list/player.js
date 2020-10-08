import useSWR, { useSWRInfinite } from "swr"
import { urlWith, get } from "../../client"

export const useList = (searchParams = {}) => {
  let url = urlWith(searchParams, `/api/player`)
  const { data, error, isValidating, mutate } = useSWR(url, get)

  return {
    data,
    isLoading: !error && !data,
    isValidating,
    error,
    isError: error,
    mutate
  }
}

export const usePlayer = ({ playerCode }) => {
  const { data, error, isValidating, mutate } = useSWR(
    playerCode ? `/api/player/${playerCode}` : null,
    (url) => get(url).then((data) => data.player)
  )

  return {
    player: data,
    isLoading: !error && !data,
    isValidating,
    error,
    isError: error,
    mutate
  }
}

export const useOperations = ({ playerCode, searchParams = {} }) => {
  let url = playerCode
    ? urlWith(searchParams, `/api/player/${playerCode}/operations`)
    : null

  const { data, error, isValidating, mutate } = useSWR(url, get)

  return {
    data: data ?? {},
    isLoading: !error && !data,
    isValidating,
    error,
    isError: error,
    mutate
  }
}
