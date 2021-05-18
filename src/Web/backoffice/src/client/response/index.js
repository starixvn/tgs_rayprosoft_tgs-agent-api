import { getAcceptLang } from "../../i18n"

const Unauthorized = 401
const UnauthorizedDefaultMessage = "Unauthorized"
const UnauthorizedTrans = {
    zh: "未经授权",
    en: UnauthorizedDefaultMessage
}

export const toDataOrError = (response) => {
    if (!response?.ok) {
        if (response.status === Unauthorized) {
            location.href = "/login"
            return Promise.reject({ title: errorTitle(response) })
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
const BadRequestDefaultMessage = "Bad Request"
const BadRequestTrans = {
    zh: "错误请求",
    en: BadRequestDefaultMessage
}

const Forbidden = 403
const ForbiddenDefaultMessage = "Forbidden"
const ForbiddenTrans = {
    zh: "禁止",
    en: ForbiddenDefaultMessage
}

const NotFound = 404
const NotFoundDefaultMessage = "Not Found"
const NotFoundTrans = {
    zh: "未找到",
    en: NotFoundDefaultMessage
}

const MethodNotAllowed = 405
const MethodNotAllowedDefaultMessage = "Method Not Allowed"
const MethodNotAllowedTrans = {
    zh: "方法不允许",
    en: MethodNotAllowedDefaultMessage
}

const RequestTimeout = 408
const RequestTimeoutDefaultMessage = "Request Timeout"
const RequestTimeoutTrans = {
    zh: "请求超时",
    en: RequestTimeoutDefaultMessage
}

const InternalServerError = 500
const InternalServerErrorDefaultMessage = "Server Error"
const InternalServerErrorTrans = {
    zh: "服务器错误",
    en: InternalServerErrorDefaultMessage
}

const NotImplemented = 501

const BadGateway = 502
const BadGatewayDefaultMessage = "Bad Gateway"
const BadGatewayTrans = {
    zh: "错误的网关",
    en: BadGatewayDefaultMessage
}

const ServiceUnavailable = 503
const ServiceUnavailableDefaultMessage = "Service Unavailable"
const ServiceUnavailableTrans = {
    zh: "服务不可用",
    en: ServiceUnavailableDefaultMessage
}

const GatewayTimeout = 504
const errorTitle = (response) => {
    let locale = getAcceptLang()
    switch (response.status) {
        case BadRequest:
            return BadRequestTrans?.[locale] ?? BadRequestDefaultMessage
        case Unauthorized:
            return UnauthorizedTrans?.[locale] ?? UnauthorizedDefaultMessage
        case Forbidden:
            return ForbiddenTrans?.[locale] ?? ForbiddenDefaultMessage
        case NotFound:
            return NotFoundTrans?.[locale] ?? NotFoundDefaultMessage
        case MethodNotAllowed:
            return (
                MethodNotAllowedTrans?.[locale] ??
                MethodNotAllowedDefaultMessage
            )
        case RequestTimeout:
            return RequestTimeoutTrans?.[locale] ?? RequestTimeoutDefaultMessage
        case InternalServerError:
        case NotImplemented:
            return (
                InternalServerErrorTrans?.[locale] ??
                InternalServerErrorDefaultMessage
            )
        case BadGateway:
            return BadGatewayTrans?.[locale] ?? BadGatewayDefaultMessage
        case ServiceUnavailable:
            return ServiceUnavailableTrans?.[locale] ?? ServiceUnavailableTrans
        case GatewayTimeout:
            return BadGatewayTrans?.[locale] ?? BadGatewayDefaultMessage
        default:
            return (
                InternalServerErrorTrans?.[locale] ??
                InternalServerErrorDefaultMessage
            )
    }
}
