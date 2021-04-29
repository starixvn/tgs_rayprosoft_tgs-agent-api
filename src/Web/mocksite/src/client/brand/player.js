import useSWR, { cache } from "swr"
import { toDataOrError } from "../response"
import { useBrandToken } from "./index"
import { intValue as playerTypeIntValue } from "../../brand/player/PlayerType"
import { fromInt } from "../../brand/player/PlayerStatus"
import { handleError } from "../Error"
import { DefaultSettings as PaginationDefaultSettings } from "../pagination"

export const useBrandPlayerList = (formData) => {
    let page = formData?.page ?? PaginationDefaultSettings?.page
    let rows = formData?.rows ?? PaginationDefaultSettings?.rows

    let keys = formData
        ? `/global-api/brand/players/mocksite?brandcode=${
              formData.brandCode
          }&currency=${formData.currency}&playertype=${playerTypeIntValue(
              formData.playerType
          )}&page=${page}&rows=${rows}&showAll=false`
        : undefined
    let { data: result, error, ...rest } = useSWR(keys, (url) =>
        fetch(url)
            .then(toDataOrError)
            .then((data) => ({
                ...data,
                items: data.items.map((player) => ({
                    ...player,
                    status: fromInt(player.status)
                }))
            }))
    )

    return {
        result,
        error,
        isLoading: !result && !error && keys,
        ...rest
    }
}

export const useCreatePlayer = ({ brandCode, currency }, formData) => {
    let { token: brandToken, ...brandTokenHook } = useBrandToken({
        brandCode,
        currency
    })

    let keys =
        brandCode && currency && formData
            ? [`/api/Player/create`, formData]
            : undefined

    let { data: result, error, ...rest } = useSWR(keys, (url, formData) => {
        return fetch(url, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                Authorization: `Bearer ${brandToken}`
            },
            body: JSON.stringify(formData)
        })
            .then(toDataOrError)
            .catch(handleError({ brandTokenHook }))
    })

    return {
        result,
        error,
        isLoading: keys && !result && !error,
        ...rest
    }
}

export const useCheckPlayerUsername = ({ brandCode, currency }, userName) => {
    let { token: brandToken, ...brandTokenHook } = useBrandToken({
        brandCode,
        currency
    })

    let keys =
        brandToken && userName
            ? [
                  `/api/Player/checkUsername?username=${userName}`,
                  userName,
                  brandCode,
                  currency
              ]
            : undefined

    let { data: result, error, ...rest } = useSWR(keys, (url, formData) => {
        return fetch(url, {
            headers: {
                "Content-Type": "application/json",
                Authorization: `Bearer ${brandToken}`
            }
        })
            .then(toDataOrError)
            .catch(handleError({ brandTokenHook }))
    })

    return {
        result,
        error,
        ...rest
    }
}

const DefaultPassword = "TgsP@ssw0rd"
const playerTokenCacheKey = ({ brandCode, currency, username, platformtype }) =>
    `playerToken:${brandCode}:${currency}:${username}:${platformtype}`

const deletePlayerTokenCache = ({
    brandCode,
    currency,
    username,
    platformtype
}) =>
    cache.delete(
        playerTokenCacheKey({ brandCode, currency, username, platformtype })
    )

const playerTokenFormDataCacheKey = ({
    brandCode,
    currency,
    username,
    platformtype
}) => `playerTokenFormData:${brandCode}:${currency}:${username}:${platformtype}`

export const usePlayerToken = (
    { brandCode, currency, username, platformtype } = {},
    data
) => {
    let { token: brandToken, ...brandTokenHook } = useBrandToken({
        brandCode,
        currency
    })

    let formDataCacheKey = playerTokenFormDataCacheKey({
        brandCode,
        currency,
        username,
        platformtype
    })
    data = data ??
        cache.get(formDataCacheKey) ?? {
            username,
            password: DefaultPassword,
            platformtype,
            bypassCaptcha: true,
            captcha: "captcha",
            answer: "answer"
        }

    let body =
        Number.isFinite(platformtype) && data?.username && data?.password
            ? JSON.stringify(data)
            : undefined
    let keys =
        brandCode && brandToken && username && body
            ? [`/api/player/login`, brandCode, username, body]
            : undefined

    const ResultCacheKey = playerTokenCacheKey({
        brandCode,
        currency,
        username,
        platformtype
    })
    let cached = cache.get(ResultCacheKey)

    let { data: result, error, mutate, ...rest } = useSWR(
        keys,
        (url, brandCode, username, body) => {
            return fetch(url, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    Authorization: `Bearer ${brandToken}`
                },
                body
            })
                .then(toDataOrError)
                .then((result) => {
                    let autoLoginFormData = JSON.parse(body)
                    cache.set(formDataCacheKey, {
                        username: autoLoginFormData.username,
                        password: autoLoginFormData.password,
                        platformtype: autoLoginFormData.platformtype,
                        bypassCaptcha: true,
                        captcha: "captcha",
                        answer: "answer",
                        formDataFromCache: true
                    })
                    cache.set(ResultCacheKey, result)
                    return result
                })
                .catch(handleError({ brandTokenHook }))
        },
        {
            initialData: keys && cached?.token ? cached : undefined
        }
    )

    return {
        token: result?.token,
        isLoading: keys && !result && !error,
        error,
        mutate: (data, shouldRevalidate) => {
            cache.delete(keys)
            cache.delete(ResultCacheKey)
            mutate(data, shouldRevalidate)
        },
        deleteCache: () => {
            cache.delete(keys)
            cache.delete(ResultCacheKey)
        },
        clearCache: () => cache.clear(),
        ...rest
    }
}

export const useLogout = (
    { brandCode, currency, username, platformtype },
    playerToken
) => {
    let { token: brandToken, ...brandTokenHook } = useBrandToken({
        brandCode,
        currency
    })

    let keys =
        brandToken && playerToken
            ? [`/api/player/logout`, brandToken, playerToken]
            : undefined
    if (keys) {
        deletePlayerTokenCache({ brandCode, currency, username, platformtype })
    }
    let { data: result, ...rest } = useSWR(
        keys,
        (url, brandToken, playerToken) =>
            fetch(url, {
                method: "DELETE",
                headers: {
                    "Content-Type": "application/json",
                    Authorization: `Bearer ${brandToken}`,
                    Token: playerToken
                }
            })
                .then(toDataOrError)
                .catch(handleError({ brandTokenHook }))
    )

    return {
        result,
        isLoading: keys && !result,
        deleteCache: () => {
            cache.delete(keys)
        },
        clearCache: () => cache.clear(),
        ...rest
    }
}

export const useCaptcha = ({ brandCode, currency }) => {
    let { token: brandToken, ...brandTokenHook } = useBrandToken({
        brandCode,
        currency
    })
    let keys = brandToken ? [`/api/player/captcha`, brandCode] : undefined

    let { data = {}, ...rest } = useSWR(keys, (url, brandCode) =>
        fetch(url, {
            headers: {
                "Content-Type": "application/json",
                Authorization: `Bearer ${brandToken}`
            }
        })
            .then(toDataOrError)
            .catch(handleError({ brandTokenHook }))
    )

    return {
        ...data,
        ...rest
    }
}

export const usePlayerInfo = ({
    brandCode,
    currency,
    username,
    platformtype
} = {}) => {
    let { token: brandToken, ...brandTokenHook } = useBrandToken({
        brandCode,
        currency
    })
    let { token: playerToken, ...playerTokenHook } = usePlayerToken({
        brandCode,
        username,
        currency,
        platformtype
    })

    let keys =
        brandToken && playerToken
            ? [`/api/player/info`, brandCode, currency, username]
            : undefined

    let { data, ...rest } = useSWR(
        keys,
        (url, brandCode, currency, username) => {
            return fetch(url, {
                headers: {
                    "Content-Type": "application/json",
                    Authorization: `Bearer ${brandToken}`,
                    Token: playerToken
                }
            })
                .then(toDataOrError)
                .catch(handleError({ brandTokenHook, playerTokenHook }))
        }
    )

    return {
        data: data?.data,
        ...rest
    }
}

export const useUpdatePlayerInfo = (
    { brandCode, currency, username, platformtype } = {},
    formData
) => {
    let { token: brandToken, ...brandTokenHook } = useBrandToken({
        brandCode,
        currency
    })
    let { token: playerToken, ...playerTokenHook } = usePlayerToken({
        brandCode,
        username,
        currency,
        platformtype
    })

    let keys =
        brandToken && playerToken && formData
            ? [`/api/player/update`, brandCode, currency, username, formData]
            : undefined

    let { data: result, error, ...rest } = useSWR(
        keys,
        (url, brandCode, currency, username, formData) => {
            return fetch(url, {
                method: "PUT",
                headers: {
                    "Content-Type": "application/json",
                    Authorization: `Bearer ${brandToken}`,
                    Token: playerToken
                },
                body: JSON.stringify(formData)
            })
                .then(toDataOrError)
                .catch(handleError({ brandTokenHook, playerTokenHook }))
        }
    )

    return {
        result,
        error,
        isLoading: keys && !result && !error,
        ...rest
    }
}

export const useChangePlayerPassword = (
    { brandCode, currency, username, platformtype } = {},
    formData
) => {
    let { token: brandToken, ...brandTokenHook } = useBrandToken({
        brandCode,
        currency
    })
    let { token: playerToken, ...playerTokenHook } = usePlayerToken({
        brandCode,
        username,
        currency,
        platformtype
    })

    let keys =
        brandToken && playerToken && formData
            ? [
                  `/api/player/changePassword`,
                  formData,
                  brandCode,
                  currency,
                  username,
                  platformtype
              ]
            : undefined

    let { data: result, error, ...rest } = useSWR(keys, (url, formData) => {
        return fetch(url, {
            method: "PUT",
            headers: {
                "Content-Type": "application/json",
                Authorization: `Bearer ${brandToken}`,
                Token: playerToken
            },
            body: JSON.stringify(formData)
        })
            .then(toDataOrError)
            .catch(handleError({ brandTokenHook, playerTokenHook }))
    })

    return {
        result,
        error,
        isLoading: keys && !result && !error,
        ...rest
    }
}

export const usePaymentList = ({
    brandCode,
    currency,
    username,
    platformtype
} = {}) => {
    let { token: brandToken, ...brandTokenHook } = useBrandToken({
        brandCode,
        currency
    })
    let { token: playerToken, ...playerTokenHook } = usePlayerToken({
        brandCode,
        username,
        currency,
        platformtype
    })

    let keys =
        brandToken && playerToken
            ? [`/api/deposit`, brandCode, currency, username, platformtype]
            : undefined

    let { data: result, error, ...rest } = useSWR(keys, (url) => {
        return fetch(url, {
            headers: {
                "Content-Type": "application/json",
                Authorization: `Bearer ${brandToken}`,
                Token: playerToken
            }
        })
            .then(toDataOrError)
            .catch(handleError({ brandTokenHook, playerTokenHook }))
    })

    return {
        result,
        error,
        isLoading: keys && !result && !error,
        ...rest
    }
}

export const useApplyDeposit = (
    { brandCode, currency, username, platformtype } = {},
    formData
) => {
    let { token: brandToken, ...brandTokenHook } = useBrandToken({
        brandCode,
        currency
    })
    let { token: playerToken, ...playerTokenHook } = usePlayerToken({
        brandCode,
        username,
        currency,
        platformtype
    })

    let keys =
        brandToken && playerToken && formData
            ? [
                  `/api/deposit/manual`,
                  formData,
                  brandCode,
                  currency,
                  username,
                  platformtype
              ]
            : undefined

    let { data: result, error, ...rest } = useSWR(keys, (url, formData) => {
        return fetch(url, {
            method: "PUT",
            headers: {
                "Content-Type": "application/json",
                Authorization: `Bearer ${brandToken}`,
                Token: playerToken
            },
            body: JSON.stringify(formData)
        })
            .then(toDataOrError)
            .catch(handleError({ brandTokenHook, playerTokenHook }))
    })

    return {
        result,
        error,
        isLoading: keys && !result && !error,
        ...rest
    }
}

export const useUploadDepositProof = (
    { brandCode, currency, username, platformtype, depositFormId } = {},
    formData
) => {
    let { token: brandToken, ...brandTokenHook } = useBrandToken({
        brandCode,
        currency
    })
    let { token: playerToken, ...playerTokenHook } = usePlayerToken({
        brandCode,
        username,
        currency,
        platformtype
    })

    let keys =
        brandToken && playerToken && depositFormId && formData
            ? [
                  `/api/deposit/manual/${depositFormId}`,
                  formData,
                  brandCode,
                  currency,
                  username,
                  platformtype
              ]
            : undefined

    let { data: result, error, ...rest } = useSWR(keys, (url, formData) => {
        return fetch(url, {
            method: "POST",
            headers: {
                Authorization: `Bearer ${brandToken}`,
                Token: playerToken
            },
            body: formData
        })
            .then(toDataOrError)
            .catch(handleError({ brandTokenHook, playerTokenHook }))
    })

    return {
        result,
        error,
        isLoading: keys && !result && !error,
        ...rest
    }
}

export const useBankInfoList = ({
    brandCode,
    currency,
    username,
    platformtype
}) => {
    let { token: brandToken, ...brandTokenHook } = useBrandToken({
        brandCode,
        currency
    })
    let { token: playerToken, ...playerTokenHook } = usePlayerToken({
        brandCode,
        username,
        currency,
        platformtype
    })

    let keys =
        brandToken && playerToken
            ? [`/api/bankinfo`, brandCode, currency, username, platformtype]
            : undefined

    let { data, error, ...rest } = useSWR(keys, (url) => {
        return fetch(url, {
            headers: {
                "Content-Type": "application/json",
                Authorization: `Bearer ${brandToken}`,
                Token: playerToken
            }
        })
            .then(toDataOrError)
            .catch(handleError({ brandTokenHook, playerTokenHook }))
    })

    return {
        data: data?.data,
        error,
        isLoading: keys && !data && !error,
        ...rest
    }
}

export const useAddBankInfo = (
    { brandCode, currency, username, platformtype },
    formData
) => {
    let { token: brandToken, ...brandTokenHook } = useBrandToken({
        brandCode,
        currency
    })
    let { token: playerToken, ...playerTokenHook } = usePlayerToken({
        brandCode,
        username,
        currency,
        platformtype
    })

    let keys =
        brandToken && playerToken && formData
            ? [
                  `/api/bankinfo`,
                  formData,
                  brandCode,
                  currency,
                  username,
                  platformtype
              ]
            : undefined

    let { data: result, error, ...rest } = useSWR(keys, (url, formData) => {
        return fetch(url, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                Authorization: `Bearer ${brandToken}`,
                Token: playerToken
            },
            body: JSON.stringify(formData)
        })
            .then(toDataOrError)
            .catch(handleError({ brandTokenHook, playerTokenHook }))
    })

    return {
        result,
        error,
        isLoading: keys && !result && !error,
        ...rest
    }
}

export const useDeleteBankInfo = (
    { brandCode, currency, username, platformtype },
    bankInfoId
) => {
    let { token: brandToken, ...brandTokenHook } = useBrandToken({
        brandCode,
        currency
    })
    let { token: playerToken, ...playerTokenHook } = usePlayerToken({
        brandCode,
        username,
        currency,
        platformtype
    })

    let keys =
        brandToken && playerToken && bankInfoId
            ? [
                  `/api/bankinfo/${bankInfoId}`,
                  brandCode,
                  currency,
                  username,
                  platformtype
              ]
            : undefined

    let { data: result, error, ...rest } = useSWR(keys, (url) => {
        return fetch(url, {
            method: "DELETE",
            headers: {
                "Content-Type": "application/json",
                Authorization: `Bearer ${brandToken}`,
                Token: playerToken
            }
        })
            .then(toDataOrError)
            .catch(handleError({ brandTokenHook, playerTokenHook }))
    })

    return {
        result,
        error,
        isLoading: keys && !result && !error,
        ...rest
    }
}

export const useApplyWithdraw = (
    { brandCode, currency, username, platformtype } = {},
    formData
) => {
    let { token: brandToken, ...brandTokenHook } = useBrandToken({
        brandCode,
        currency
    })
    let { token: playerToken, ...playerTokenHook } = usePlayerToken({
        brandCode,
        username,
        currency,
        platformtype
    })

    let keys =
        brandToken && playerToken && formData
            ? [
                  `/api/withdraw/manual`,
                  formData,
                  brandCode,
                  currency,
                  username,
                  platformtype
              ]
            : undefined

    let { data: result, error, ...rest } = useSWR(keys, (url, formData) => {
        return fetch(url, {
            method: "PUT",
            headers: {
                "Content-Type": "application/json",
                Authorization: `Bearer ${brandToken}`,
                Token: playerToken
            },
            body: JSON.stringify(formData)
        })
            .then(toDataOrError)
            .catch(handleError({ brandTokenHook, playerTokenHook }))
    })

    return {
        result,
        error,
        isLoading: keys && !result && !error,
        ...rest
    }
}
