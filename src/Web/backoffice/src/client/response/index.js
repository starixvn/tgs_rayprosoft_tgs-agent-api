import { getAcceptLang } from "../../i18n"

const UnauthorizedStatus = 401
const UnauthorizedStatusTrans = {
    zh: "未经授权",
    en: "Unauthorized"
}

export const toDataOrError = (response) => {
    if (!response?.ok) {
        if (response.status === UnauthorizedStatus) {
            location.href = "/login"
        } else {
            return response
                .json()
                .catch((error) => {
                    return Promise.reject({
                        title: errorTitle(response)
                    })
                })
                .then((data) => {
                    let { title, message, errors = [], ...rest } = data
                    return Promise.reject({
                        title: title ?? message ?? errorTitle(response),
                        errors:
                            errors?.length > 0
                                ? Object.values(errors)
                                : [JSON.stringify(rest)]
                    })
                })
        }
    }

    return response.json().then((data) => {
        if (!data?.valid && data?.message) {
            return Promise.reject({
                title: data.message
            })
        }

        return data
    })
}

const BadRequest = 400
const BadRequestTrans = {
    zh: "错误请求",
    en: "Bad Request"
}

const Forbidden = 403
const ForbiddenTrans = {
    zh: "禁止",
    en: "Forbidden"
}

const NotFound = 404
const MethodNotAllowed = 405
const RequestTimeout = 408
const InternalServerError = 500
const NotImplemented = 501
const BadGateway = 502
const ServiceUnavailable = 503
const GatewayTimeout = 504
const errorTitle = (response) => {
    let locale = getAcceptLang()
    console.log("locale", locale)
    switch (response.status) {
        case BadRequest:
            return "Bad Request"
        case Forbidden:
            return "Forbidden"
        case NotFound:
            return "Not Found"
        case MethodNotAllowed:
            return "Method Not Allowed"
        case RequestTimeout:
            return "Request Timeout"
        case InternalServerError:
        case NotImplemented:
            return "Server Error"
        case BadGateway:
            return "Bad Gateway"
        case ServiceUnavailable:
            return "Service Unavailable"
        case GatewayTimeout:
            return "Gateway Timeout"
    }

    return response.statusText ?? `Error ${response.status}`
}
