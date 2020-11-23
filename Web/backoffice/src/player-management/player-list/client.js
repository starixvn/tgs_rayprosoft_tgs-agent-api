import {
  urlWith,
  useGet,
  usePostData,
  usePutData,
  useDelete
} from "../../client"

let API = "/api/player"

export const usePlayerList = (searchParams) => {
  let url = searchParams ? urlWith(searchParams, API) : ""
  return useGet(url)
}

export const usePlayer = (playerCode) => {
  let url = playerCode ? `${API}/${playerCode}` : ""
  let { data, ...rest } = useGet(url)
  return {
    data: data?.player,
    ...rest
  }
}

export const useEditPlayerInfo = (playerCode, data) => {
  let url = playerCode ? `${API}/${playerCode}/info` : ""
  return usePutData(url, data)
}

export const useEditPlayerContact = (playerCode, data) => {
  let url = playerCode ? `${API}/${playerCode}/contact` : ""
  return usePutData(url, data)
}

export const useAddPlayerBank = (playerCode, data) => {
  let url = playerCode ? `${API}/${playerCode}/bank` : ""
  return usePostData(url, data)
}

export const useEditPlayerBank = ({ playerCode, id } = {}, data) => {
  let url = playerCode && id ? `${API}/${playerCode}/bank/${id}` : ""
  return usePutData(url, data)
}

export const useDeletePlayerBank = (playerCode, id) => {
  let url = playerCode && id ? `${API}/${playerCode}/bank/${id}` : ""
  return useDelete(url)
}

export const useEditPlayerPassword = (playerCode, data) => {
  let url = playerCode ? `${API}/${playerCode}/password` : ""
  return usePutData(url, data)
}

export const useOperations = (playerCode, searchParams) => {
  let url = playerCode
    ? urlWith(searchParams, `${API}/${playerCode}/operations`)
    : ""
  return useGet(url)
}

export const useBettings = (playerCode, searchParams) => {
  let url = playerCode
    ? urlWith(searchParams, `${API}/${playerCode}/bettings`)
    : ""
  return useGet(url)
}
