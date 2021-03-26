import React, { useState, useEffect, useMemo, useRef, useContext } from "react"
import { css, Global } from "@emotion/react"
import {
    Spinner,
    Stack,
    Flex,
    Box,
    Card,
    Set,
    Button,
    Text,
    Dialog,
    Modal,
    Table,
    useClassName
} from "bumbag"
import { FieldValues } from "../../layout/Grid"
import { Trans, FormattedDateTime, FormattedNumber, useTrans } from "../../i18n"
import AlertDialog from "../../dialog/Alert"
import { SubmitCancel } from "../../button/ActionButtons"
import { Tab as _Tab, TabPanel } from "../../tab"
import { FormProvider, useForm, useFormContext } from "react-hook-form"
import Input from "../../input/Input"
import { useAgent, useUpdateAgent } from "./client"
import clone from "clone-deep"

const Key = "AgentContactInfo"

export const AgentContactInfoTab = ({ ...props }) => {
    return (
        <_Tab tabId={Key} {...props}>
            <Trans zh="联系信息" en="Contact Info" />
        </_Tab>
    )
}

export const AgentContactInfoPanel = ({ readonly = false, ...props }) =>
    readonly ? <ViewPanel {...props} /> : <EditPanel {...props} />

const EditPanel = ({ record, onSubmitted, onClickClose, ...props }) => {
    let formMethods = useForm({
        defaultValues: clone(record)
    })

    let agentId = record?.id
    let [formData, setSubmit] = useState()
    let { isLoading, result, error, mutate, deleteCache } = useUpdateAgent(
        agentId,
        formData
    )

    useEffect(() => {
        if (result && !error) {
            onSubmitted && onSubmitted()
        }
    }, [result])

    const onSubmit = (data) => {
        setSubmit({
            ...record,
            updateType: "contact",
            ...data
        })
    }

    const reset = () => {
        deleteCache()
        mutate(undefined, false)
        setSubmit(undefined)
    }

    return (
        <TabPanel tabId={Key} {...props}>
            <FormProvider {...formMethods}>
                <form onSubmit={formMethods.handleSubmit(onSubmit)}>
                    <Stack>
                        <FieldValues>
                            <MobileBox>
                                <Input name="phone" />
                            </MobileBox>

                            <EmailBox>
                                <Input name="email" />
                            </EmailBox>

                            <QqBox>
                                <Input name="qq" />
                            </QqBox>

                            <WeChatBox>
                                <Input name="wechat" />
                            </WeChatBox>
                        </FieldValues>
                        <Box alignX="right">
                            <SubmitCancel
                                isLoading={isLoading}
                                onClickCancel={onClickClose}
                            />
                        </Box>
                    </Stack>
                </form>
            </FormProvider>
            {error && <AlertDialog {...error} onClickClose={reset} />}
        </TabPanel>
    )
}

const ViewPanel = ({ record = {}, ...props }) => {
    return (
        <TabPanel tabId={Key} {...props}>
            <FieldValues spacing="major-2">
                <MobileBox value={record?.phone} />

                <EmailBox value={record?.email} />

                <QqBox value={record?.qq} />

                <WeChatBox value={record?.wechat} />
            </FieldValues>
        </TabPanel>
    )
}

const MobileBox = ({ value = "", children }) => {
    return (
        <>
            <Box alignY="center">
                <Trans zh="手机号码" en="Mobile" />
            </Box>

            {children ? children : <Text>{value}</Text>}
        </>
    )
}

const EmailBox = ({ value = "", children }) => {
    return (
        <>
            <Box alignY="center">
                <Trans zh="邮箱" en="Email" />
            </Box>

            {children ? children : <Text>{value}</Text>}
        </>
    )
}

const QqBox = ({ value = "", children }) => {
    return (
        <>
            <Box alignY="center">
                <Trans zh="QQ" en="QQ" />
            </Box>

            {children ? children : <Text>{value}</Text>}
        </>
    )
}

const WeChatBox = ({ value = "", children }) => {
    return (
        <>
            <Box alignY="center">
                <Trans zh="微信号" en="WeChat" />
            </Box>

            {children ? children : <Text>{value}</Text>}
        </>
    )
}
