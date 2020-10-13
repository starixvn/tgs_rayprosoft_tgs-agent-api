import useSWR from "swr"
import { urlWith, get, put } from "../../client"

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

export const useEditPlayerInfo = ({ playerCode, data }) => {
  const { data: result, error, isValidating, mutate } = useSWR(
    playerCode && data ? `/api/player/${playerCode}/info` : null,
    (url) => put(url, { data })
  )

  return {
    result,
    isLoading: !error && !result,
    isValidating,
    error,
    isError: error,
    mutate
  }
}

export const useEditPlayerContact = ({ playerCode, data }) => {
  const { data: result, error, isValidating, mutate } = useSWR(
    playerCode && data ? `/api/player/${playerCode}/contact` : null,
    (url) => put(url, { data })
  )

  return {
    result,
    isLoading: !error && !result,
    isValidating,
    error,
    isError: error,
    mutate
  }
}

export const useEditPlayerPassword = ({ player }) => {
  const { data: result, error, isValidating, mutate } = useSWR(
    player ? `/api/player/${player.playerCode}/password` : null,
    (url) => put(url, { data: player })
  )

  return {
    result,
    isLoading: !error && !result,
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
