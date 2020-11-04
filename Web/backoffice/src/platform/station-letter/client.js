import useSWR, { cache } from "swr"
import client, {
  urlWith,
  get,
  put,
  post,
  useGet,
  useCreatePost,
  useUpdatePut
} from "../../client"

const API = "/api/stationletter"

export const useStationLetterList = (searchParams = {}) => {
  let url = urlWith(searchParams, API)
  return useGet(url)
}

export const useVerifyStationLetter = ({ id, type, data } = {}) => {
  let url = `${API}/${id}/${type}`
  return useUpdatePut(url, data)
}

export const useAddStationLetter = ({ data } = {}) => {
  return useCreatePost(API, data)
}
