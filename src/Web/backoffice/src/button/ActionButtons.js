import React, { useState, useEffect, useMemo, useRef, useContext } from "react"
import { css, Global } from "@emotion/react"
import { ActionButtons as _ActionButtons } from "bumbag"
import { Trans, useTrans } from "../i18n"

const Submit = {
    zh: "提交",
    en: "Submit"
}

const Confirm = {
    zh: "确定",
    en: "Submit"
}

const Cancel = {
    zh: "取消",
    en: "Cancel"
}

const Close = {
    zh: "关闭",
    en: "Close"
}

const Delete = {
    zh: "删除",
    en: "Delete"
}

const Add = {
    zh: "新增",
    en: "Add"
}

export const SubmitText = () => Trans(Submit)

export const useSubmitText = () => useTrans(Submit)

export const CancelText = () => Trans(Cancel)

export const useCancelText = () => useTrans(Cancel)

export const ConfirmText = () => Trans(Confirm)

export const useConfirmText = () => useTrans(Confirm)

export const DeleteText = () => <Trans {...Delete} />

export const AddText = () => <Trans {...Add} />

export const CloseText = () => useTrans(Close)

export const SubmitCancel = ({ cancelText, submitText, ...props }) => {
    return (
        <_ActionButtons
            cancelText={cancelText ?? <CancelText />}
            submitText={submitText ?? <SubmitText />}
            {...props}
        />
    )
}

export const ConfirmCancel = ({ cancelText, submitText, ...props }) => {
    return (
        <_ActionButtons
            cancelText={cancelText ?? <CancelText />}
            submitText={submitText ?? <ConfirmText />}
            {...props}
        />
    )
}

export default SubmitCancel
