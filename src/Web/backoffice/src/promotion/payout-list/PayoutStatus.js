import React from "react"
import { css } from "@emotion/react"
import { Spinner } from "bumbag"
import { getTrans, Trans, useLocale, useTrans } from "../../i18n"
import { Switch } from "../../switch/Switch"
import { Select } from "../../select/Select"

export const PayoutStatusLabel = ({ value }) => {
    let sentLabel = useTrans({
        zh: "已派发",
        en: "Sent"
    })

    let notSentLabel = useTrans({
        zh: "未派发",
        en: "Not Sent"
    })

    let isSent = !!value
    return isSent ? sentLabel : notSentLabel
}
