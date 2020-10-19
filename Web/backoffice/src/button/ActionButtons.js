import React, { useState, useEffect, useMemo, useRef, useContext } from "react"
import { css, Global } from "@emotion/core"
import {
  ActionButtons as _ActionButtons
} from "bumbag"
import { useTrans } from "../i18n"

export const SubmitCancel = ({ cancelText, submitText, ...props}) => {
    let defaultCancelText = useTrans({
        zh: "取消",
        en: "Cancel"
    })

    let defaultSubmitText = useTrans({
        zh: "提交",
        en: "Submit"
    })

    return (
        <_ActionButtons 
            cancelText={cancelText ?? defaultCancelText}  
            submitText={submitText ?? defaultSubmitText}
            {...props}/>
    )
}

export const ConfirmCancel = ({ cancelText, submitText, ...props}) => {
    let defaultCancelText = useTrans({
        zh: "取消",
        en: "Cancel"
    })

    let defaultSubmitText = useTrans({
        zh: "确定",
        en: "Submit"
    })

    return (
        <_ActionButtons 
            cancelText={cancelText ?? defaultCancelText}  
            submitText={submitText ?? defaultSubmitText}
            {...props}/>
    )
}

export default SubmitCancel