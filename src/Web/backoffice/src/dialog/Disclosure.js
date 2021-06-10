import React, { useState, useEffect, useMemo, useRef, forwardRef } from "react"
import { Button, Modal, css, Icon } from "bumbag"
import { Trans } from "../i18n"
import { faCog } from "@fortawesome/free-solid-svg-icons"

export const Disclosure = forwardRef(
    ({ use = Button, children, ...props }, ref) => (
        <Modal.Disclosure
            ref={ref}
            use={use}
            flex="unset"
            css={css`
                white-space: nowrap;
            `}
            {...props}
        >
            {children}
        </Modal.Disclosure>
    )
)

export const ViewDisclosure = forwardRef((props, ref) => (
    <Disclosure ref={ref} {...props}>
        <Trans zh="查看" en="View" />
    </Disclosure>
))

export const EditDisclosure = forwardRef((props, ref) => (
    <Disclosure ref={ref} {...props}>
        <Trans zh="编辑" en="Edit" />
    </Disclosure>
))

export const AddDisclosure = forwardRef((props, ref) => (
    <Disclosure ref={ref} {...props}>
        <Trans zh="新增" en="Add" />
    </Disclosure>
))

export const NewDisclosure = forwardRef((props, ref) => (
    <Disclosure ref={ref} {...props}>
        <Trans zh="新增" en="New" />
    </Disclosure>
))

export const DeleteDisclosure = forwardRef(
    ({ palette = "danger", variant = "outlined", ...props }, ref) => (
        <Disclosure ref={ref} palette={palette} variant={variant} {...props}>
            <Trans zh="删除" en="Delete" />
        </Disclosure>
    )
)

export const DetailDisclosure = forwardRef((props, ref) => (
    <Disclosure ref={ref} {...props}>
        <Trans zh="详情" en="Detail" />
    </Disclosure>
))

export const VerifyDisclosure = forwardRef((props, ref) => (
    <Disclosure ref={ref} {...props}>
        <Trans zh="审核" en="Verify" />
    </Disclosure>
))

export const ConfigIconDisclosure = forwardRef((props, ref) => (
    <Disclosure ref={ref} {...props}>
        <Icon icon={faCog} type="font-awesome" />
    </Disclosure>
))

export default Disclosure
