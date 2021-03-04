import useSWR, { cache } from "swr"
import {
    urlWith,
    useGet,
    usePostData,
    usePut,
    usePutData,
    usePostFormData
} from "../../client"

const API = "/api/round"

export const useGameRoundList = (searchParams) => {
    let url = searchParams ? urlWith(searchParams, API) : ""
    return useGet(url)
}

export const useGameRound = (
    id,
    { gameProviderCode, playerCode, language = "en-US" }
) => {
    let url =
        id && gameProviderCode && playerCode
            ? urlWith(
                  {
                      gameProviderCode: gameProviderCode,
                      playercode: playerCode,
                      language
                  },
                  `${API}/${id}`
              )
            : ""

    return useGet(url)
}
