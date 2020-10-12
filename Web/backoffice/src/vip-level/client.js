import useSWR from "swr"
import { urlWith, get } from "../client"

export const useVipLevels = () => {
    const { data, error, isValidating, mutate } = useSWR(
        "/api/vipLevel",
        (url) => get(url).then(({ items }) => items)
    )

    return {
        items: data ?? [],
        isLoading: !error && !data,
        isValidating,
        error,
        isError: error,
        mutate
    }
}
