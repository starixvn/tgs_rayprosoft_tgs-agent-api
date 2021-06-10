import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import {
    Box,
    Dialog,
    useModalState,
    Set,
    Level,
    Heading,
    Button,
    Flex,
    Text
} from "bumbag"
import { View as ViewMenu, Update as UpdateMenu } from "./menu"
import { useIsDisplayMenuByRoute, useUserBrandId } from "../../user/context"
import LoadingBackdrop from "../../dialog/LoadingBackdrop"
import AlertDialog from "../../dialog/Alert"
import { useAddAgent, useAgent, useUpdateAgent } from "./client"
import {
    Trans,
    FormattedDateTime,
    FormattedNumber,
    useTrans,
    FormattedNumberDecimal
} from "../../i18n"
import Disclosure, {
    AddDisclosure,
    EditDisclosure,
    ViewDisclosure
} from "../../dialog/Disclosure"
import { Tabs, TabList, useTabState } from "../../tab"
import { AgentInfoKey, AgentInfoPanel, AgentInfoTab } from "./AgentInfo"
import { AgentContactInfoPanel, AgentContactInfoTab } from "./AgentContactInfo"
import { AgentBankInfoPanel, AgentBankInfoTab } from "./AgentBankInfo"
import {
    AgentOperationListPanel,
    AgentOperationListTab
} from "./AgentOperationList"
import { AgentTeamInfoPanel, AgentTeamInfoTab } from "./AgentTeamInfo"
import { AgentPasswordPanel, AgentPasswordTab } from "./AgentPassword"
import FormDialog from "../../form/FormDialog"
import {
    CloseButtonFooter,
    ConfirmCancelFooter
} from "../../form/ConfirmButtonFooter"
import { FieldValues, TwoColumns } from "../../layout/Grid"
import { NormalLabel } from "../../form/Label"
import Input, { InputField } from "../../input/Input"
import { PasswordInputField } from "../../input/PasswordInput"
import { useFormContext, useWatch } from "react-hook-form"
import AgentLevel, {
    AgentLevelLabel,
    AgentLevelSelect,
    isAgentParentRequired
} from "../AgentLevel"
import AgentType, {
    AgentTypeLabel,
    AgentTypeSelect,
    isAgentTypeCash,
    isAgentTypeCredit
} from "../AgentType"
import Textarea from "../../textarea"
import AgentUpdateType from "./AgentUpdateType"
import { AgentDividendPercentageInputField } from "../AgentDividendPercentage"
import {
    AgentLiveDealerRebatePercentageInputField,
    AgentLiveDealerRebateText
} from "../AgentLiveDealerRebatePercentage"
import {
    AgentEgameRebatePercentageInputField,
    AgentEgameRebateText
} from "../AgentEgameRebatePercentage"
import {
    AgentCardGameRebatePercentageInputField,
    AgentCardGameRebateText
} from "../AgentCardGameRebatePercentage"
import {
    AgentLotteryRebatePercentageInputField,
    AgentLotteryRebateText
} from "../AgentLotteryRebatePercentage"
import {
    AgentSportsRebatePercentageInputField,
    AgentSportsRebateText
} from "../AgentSportsRebatePercentage"
import {
    AgentVirtualSportsRebatePercentageInputField,
    AgentVirtualSportsRebateText
} from "../AgentVirtualSportsRebatePercentage"
import clone from "clone-deep"
import {
    AgentBalanceChangePanel,
    AgentBalanceChangeTab
} from "./AgentBalanceChange"

export const AddAgentButton = ({ brandId, onSubmitted, ...props }) => {
    let dialog = useModalState()

    let isDisplay = useIsDisplayMenuByRoute([UpdateMenu])
    if (!isDisplay) {
        return null
    }

    return (
        <>
            <AddDisclosure {...dialog} />
            {dialog.visible && (
                <AddDialog
                    brandId={brandId}
                    onSubmitted={onSubmitted}
                    dialog={dialog}
                />
            )}
        </>
    )
}

const AddDialog = ({ brandId, onSubmitted, dialog }) => {
    let userBrandId = useUserBrandId()
    brandId = brandId ?? userBrandId

    let [formData, setSubmit] = useState()
    let { result, error, mutate, deleteCache, isLoading } =
        useAddAgent(formData)

    const onSubmit = (data) => {
        setSubmit({
            ...data,
            brandId
        })
    }

    const reset = () => {
        deleteCache()
        mutate(undefined, false)
        setSubmit(undefined)
    }

    let title = useTrans({
        zh: "新增代理",
        en: "Add Agent"
    })

    return (
        <FormDialog
            defaultValues={{
                levelType: AgentLevel.Chief,
                type: AgentType.Cash,
                brandId
            }}
            onSubmit={onSubmit}
            isLoading={isLoading}
            result={result}
            error={error}
            onCloseErrorAlert={reset}
            onSubmitted={onSubmitted}
            title={title}
            dialog={dialog}
            showActionButtons={false}
            footer={
                <ConfirmCancelFooter dialog={dialog} isLoading={isLoading} />
            }
            maxWidth="100vw"
            minWidth="60vw"
            placement="top"
        >
            <FormBody />
        </FormDialog>
    )
}

const FormBody = ({}) => {
    let { control } = useFormContext()

    const LevelTypeFieldName = "levelType"
    let levelType = useWatch({
        control,
        name: LevelTypeFieldName
    })
    let parentAgentRequired = isAgentParentRequired(levelType)

    const PasswordFieldName = "password"
    let password = useWatch({
        control,
        name: PasswordFieldName
    })

    const AgentTypeFieldName = "type"
    let agentType = useWatch({
        control,
        name: AgentTypeFieldName
    })
    let isCash = isAgentTypeCash(agentType)
    let isCredit = isAgentTypeCredit(agentType)
    return (
        <Set orientation="vertical" isFilled marginLeft="1rem">
            <Heading use="h5">
                <Trans zh="基本资料" en="Basic info" />
            </Heading>
            <Set
                orientation="vertical"
                marginLeft="1rem"
                marginRight="1rem"
                isFilled
            >
                <FieldValues>
                    <NormalLabel required>
                        <Trans zh="代理用户名" en="Username" />
                    </NormalLabel>
                    <Input name="username" required autoComplete="off" />

                    <NormalLabel required>
                        <Trans zh="登入密码" en="Password" />
                    </NormalLabel>
                    <PasswordInputField name={PasswordFieldName} />

                    <NormalLabel required>
                        <Trans zh="确认密码" en="Confirm password" />
                    </NormalLabel>
                    <PasswordInputField
                        name="confirmPassword"
                        matchPassword={password}
                    />

                    <NormalLabel>
                        <Trans zh="代理种类" en="Level Type" />
                    </NormalLabel>
                    <AgentLevelSelect
                        name={LevelTypeFieldName}
                        required
                        placeholder={false}
                    />

                    {!parentAgentRequired && (
                        <>
                            <NormalLabel>
                                <Trans zh="代理类型" en="Agent Type" />
                            </NormalLabel>
                            <AgentTypeSelect
                                name={AgentTypeFieldName}
                                required
                            />
                        </>
                    )}

                    {parentAgentRequired && (
                        <>
                            <NormalLabel required>
                                <Trans
                                    zh="上级用户名"
                                    en="Upper Level Username"
                                />
                            </NormalLabel>
                            <InputField name="parentUsername" required />
                        </>
                    )}

                    {isCash && (
                        <>
                            <NormalLabel required>
                                <Trans zh="代理分红" en="Profit Sharing" />
                            </NormalLabel>
                            <AgentDividendPercentageInputField />
                        </>
                    )}

                    {isCredit && (
                        <>
                            <NormalLabel required>
                                <AgentLiveDealerRebateText />
                            </NormalLabel>
                            <TwoColumns columns={["0.7fr"]}>
                                <Flex flex="1 50%">
                                    <AgentLiveDealerRebatePercentageInputField width="100%" />
                                </Flex>

                                <FieldValues flex="1 50%">
                                    <NormalLabel required>
                                        <AgentLotteryRebateText />
                                    </NormalLabel>

                                    <AgentLotteryRebatePercentageInputField />
                                </FieldValues>
                            </TwoColumns>
                        </>
                    )}

                    {isCredit && (
                        <>
                            <NormalLabel required>
                                <AgentEgameRebateText />
                            </NormalLabel>
                            <TwoColumns columns={["0.7fr"]}>
                                <Flex flex="1 50%">
                                    <AgentEgameRebatePercentageInputField width="100%" />
                                </Flex>

                                <FieldValues>
                                    <NormalLabel required>
                                        <AgentSportsRebateText />
                                    </NormalLabel>

                                    <AgentSportsRebatePercentageInputField />
                                </FieldValues>
                            </TwoColumns>
                        </>
                    )}

                    {isCredit && (
                        <>
                            <NormalLabel required>
                                <AgentCardGameRebateText />
                            </NormalLabel>
                            <TwoColumns columns={["0.7fr"]}>
                                <Flex flex="1 50%">
                                    <AgentCardGameRebatePercentageInputField width="100%" />
                                </Flex>

                                <FieldValues>
                                    <NormalLabel required>
                                        <AgentVirtualSportsRebateText />
                                    </NormalLabel>

                                    <AgentVirtualSportsRebatePercentageInputField />
                                </FieldValues>
                            </TwoColumns>
                        </>
                    )}

                    <NormalLabel required>
                        <Trans zh="备注" en="Remarks" />
                    </NormalLabel>
                    <Textarea name="remarks" required />
                </FieldValues>
            </Set>
        </Set>
    )
}

const ConfigFormBody = ({ record }) => {
    let isCash = isAgentTypeCash(record?.type)
    let isCredit = isAgentTypeCredit(record?.type)

    return (
        <Set orientation="vertical" isFilled>
            <Set
                orientation="vertical"
                marginLeft="1rem"
                marginRight="1rem"
                isFilled
            >
                <FieldValues>
                    <NormalLabel>
                        <Trans zh="代理用户名" en="Username" />
                    </NormalLabel>
                    <Text.Block alignY="center">{record?.username}</Text.Block>

                    <NormalLabel>
                        <Trans zh="真实姓名" en="Real Name" />
                    </NormalLabel>
                    <Text.Block alignY="center">{record?.realName}</Text.Block>

                    <Box alignY="center">
                        <Trans zh="上级代理" en="Upper level agent" />
                    </Box>
                    <Text.Block alignY="center">
                        {record?.parentUsername}
                    </Text.Block>

                    <NormalLabel>
                        <Trans zh="注册时间" en="Registration" />
                    </NormalLabel>
                    <Text.Block alignY="center">
                        <FormattedDateTime value={record?.registration} />
                    </Text.Block>

                    <NormalLabel>
                        <Trans zh="余额" en="Balance" />
                    </NormalLabel>
                    <Text.Block alignY="center">
                        <FormattedNumberDecimal value={record?.balance} />
                    </Text.Block>

                    <NormalLabel>
                        <Trans zh="代理种类" en="Agent Level Type" />
                    </NormalLabel>
                    <Text.Block alignY="center">
                        <AgentLevelLabel value={record?.levelType} />
                    </Text.Block>

                    <NormalLabel>
                        <Trans zh="代理类型" en="Agent Type" />
                    </NormalLabel>
                    <Text.Block alignY="center">
                        <AgentTypeLabel value={record?.type} />
                    </Text.Block>

                    {isCash && (
                        <>
                            <NormalLabel required>
                                <Trans zh="分红比例" en="Profit Sharing %" />
                            </NormalLabel>
                            <AgentDividendPercentageInputField />
                        </>
                    )}

                    {isCredit && (
                        <>
                            <NormalLabel required>
                                <AgentLiveDealerRebateText />
                            </NormalLabel>
                            <AgentLiveDealerRebatePercentageInputField />
                        </>
                    )}

                    {isCredit && (
                        <>
                            <NormalLabel required>
                                <AgentEgameRebateText />
                            </NormalLabel>
                            <AgentEgameRebatePercentageInputField />
                        </>
                    )}

                    {isCredit && (
                        <>
                            <NormalLabel required>
                                <AgentCardGameRebateText />
                            </NormalLabel>
                            <AgentCardGameRebatePercentageInputField />
                        </>
                    )}

                    {isCredit && (
                        <>
                            <NormalLabel required>
                                <AgentLotteryRebateText />
                            </NormalLabel>
                            <AgentLotteryRebatePercentageInputField />
                        </>
                    )}

                    {isCredit && (
                        <>
                            <NormalLabel required>
                                <AgentSportsRebateText />
                            </NormalLabel>
                            <AgentSportsRebatePercentageInputField />
                        </>
                    )}

                    {isCredit && (
                        <>
                            <NormalLabel required>
                                <AgentVirtualSportsRebateText />
                            </NormalLabel>
                            <AgentVirtualSportsRebatePercentageInputField />
                        </>
                    )}
                </FieldValues>
            </Set>
        </Set>
    )
}

export const ViewButton = ({ record, brandId, ...props }) => {
    let dialog = useModalState()

    let isDisplay = useIsDisplayMenuByRoute([ViewMenu])
    if (!isDisplay) {
        return null
    }

    return (
        <>
            <ViewDisclosure {...dialog} />
            {dialog.visible && (
                <TabsDialog
                    record={record}
                    brandId={brandId}
                    readonly
                    dialog={dialog}
                />
            )}
        </>
    )
}

export const EditButton = ({ brandId, record, onSubmitted, ...props }) => {
    let dialog = useModalState()

    let isDisplay = useIsDisplayMenuByRoute([UpdateMenu])
    if (!isDisplay) {
        return null
    }

    return (
        <>
            <EditDisclosure {...dialog} />
            {dialog.visible && (
                <TabsDialog
                    brandId={brandId}
                    record={record}
                    onSubmitted={onSubmitted}
                    dialog={dialog}
                />
            )}
        </>
    )
}

export const ConfigButton = ({ brandId, agentId, onSubmitted, ...props }) => {
    let dialog = useModalState()

    let isDisplay = useIsDisplayMenuByRoute([UpdateMenu])
    if (!isDisplay) {
        return null
    }

    return (
        <>
            <Disclosure {...dialog} {...props}>
                <Trans zh="配置" en="Config" />
            </Disclosure>
            {dialog.visible && (
                <ConfigDialog
                    brandId={brandId}
                    agentId={agentId}
                    onSubmitted={onSubmitted}
                    dialog={dialog}
                />
            )}
        </>
    )
}

const ConfigDialog = ({ brandId, agentId, onSubmitted, dialog }) => {
    let {
        isLoading: isLoadingAgent,
        data: agent,
        error: loadingAgentError,
        mutate: mutateAgent,
        deleteCache: deleteAgentCache
    } = useAgent(agentId)

    let [formData, setSubmit] = useState()
    let { result, error, mutate, deleteCache, isLoading } = useUpdateAgent(
        agentId,
        formData
    )

    let title = useTrans({
        zh: "比例配制",
        en: "Config Agent"
    })

    if (isLoadingAgent) {
        return <LoadingBackdrop {...dialog} />
    }

    if (loadingAgentError) {
        return <AlertDialog {...loadingAgentError} onClickClose={dialog.hide} />
    }

    const onSubmit = (data) => {
        setSubmit({
            updateType: AgentUpdateType.Info,
            brandId,
            ...agent,
            ...data
        })
    }

    const reset = () => {
        deleteCache()
        mutate(undefined, false)
        setSubmit(undefined)
    }

    return (
        <FormDialog
            defaultValues={{
                ...clone(agent),
                dividendPercentage: "",
                liveDealerRebatePercentage: "",
                slotRebatePercentage: "",
                cardGameRebatePercentage: "",
                lotteryRebatePercentage: "",
                sportsRebatePercentage: "",
                virtualSportsRebatePercentage: ""
            }}
            onSubmit={onSubmit}
            isLoading={isLoading}
            result={result}
            error={error}
            onCloseErrorAlert={reset}
            onSubmitted={() => {
                deleteAgentCache()
                mutateAgent(undefined, false)

                reset()
                onSubmitted?.()
            }}
            title={title}
            dialog={dialog}
            showActionButtons={false}
            footer={
                <ConfirmCancelFooter dialog={dialog} isLoading={isLoading} />
            }
            maxWidth="100vw"
            minWidth="60vw"
            placement="top"
        >
            <ConfigFormBody record={agent} />
        </FormDialog>
    )
}

const TabsDialog = ({
    record,
    brandId,
    readonly = false,
    dialog,
    ...props
}) => {
    let agentId = record?.id
    let tab = useTabState({
        selectedId: AgentInfoKey
    })

    let title = useTrans({
        zh: "代理详情",
        en: "Agent Info"
    })

    const { isLoading, data, error, mutate, deleteCache } = useAgent(agentId)

    if (isLoading) {
        return <LoadingBackdrop {...dialog} />
    }

    if (error) {
        return <AlertDialog {...error} {...dialog} onClickClose={dialog.hide} />
    }

    const onSubmitted = () => {
        props?.onSubmitted()
        dialog.hide()
        mutate()
    }

    const onBankSubmitted = () => {
        mutate()
    }

    const onClickClose = dialog.hide

    let editable = !readonly
    return (
        <Dialog.Modal
            title={title}
            hideOnClickOutside={false}
            showCloseButton={true}
            placement="top"
            {...dialog}
            minWidth="80vw"
            minHeight="85vh"
            css={css`
                & .bb-DialogContent {
                    min-height: 85vh;
                }
            `}
        >
            {readonly && (
                <ViewTabs
                    agentId={agentId}
                    brandId={brandId}
                    record={clone(data)}
                    tab={tab}
                />
            )}
            {editable && (
                <EditTabs
                    agentId={agentId}
                    brandId={brandId}
                    record={clone(data)}
                    tab={tab}
                    onSubmitted={onSubmitted}
                    onBankSubmitted={onBankSubmitted}
                    onClickClose={onClickClose}
                />
            )}
        </Dialog.Modal>
    )
}

const ViewTabs = ({ brandId, record, tab }) => {
    return (
        <Tabs isFitted>
            <TabList use={TabList} {...tab}>
                <AgentInfoTab {...tab} />
                <AgentContactInfoTab {...tab} />
                <AgentBankInfoTab {...tab} />
                <AgentOperationListTab {...tab} />
                <AgentTeamInfoTab {...tab} />
                <AgentBalanceChangeTab {...tab} />
            </TabList>

            <AgentInfoPanel record={record} readonly {...tab} />
            <AgentContactInfoPanel record={record} readonly {...tab} />
            <AgentBankInfoPanel record={record} readonly {...tab} />
            <AgentOperationListPanel agentId={record?.id} {...tab} />
            <AgentTeamInfoPanel agentId={record?.id} {...tab} />
            <AgentBalanceChangePanel
                brandId={brandId}
                agentId={record?.id}
                {...tab}
            />
        </Tabs>
    )
}

const EditTabs = ({
    brandId,
    agentId,
    record,
    tab,
    onSubmitted,
    onBankSubmitted,
    onClickClose
}) => {
    return (
        <Tabs isFitted>
            <TabList use={TabList} {...tab}>
                <AgentInfoTab {...tab} />
                <AgentContactInfoTab {...tab} />
                <AgentBankInfoTab {...tab} />
                <AgentPasswordTab {...tab} />
            </TabList>
            <AgentInfoPanel
                record={record}
                brandId={brandId}
                agentId={agentId}
                onSubmitted={onSubmitted}
                onClickClose={onClickClose}
                {...tab}
            />
            <AgentContactInfoPanel
                record={record}
                brandId={brandId}
                agentId={agentId}
                onSubmitted={onSubmitted}
                onClickClose={onClickClose}
                {...tab}
            />
            <AgentBankInfoPanel
                record={record}
                brandId={brandId}
                agentId={agentId}
                onSubmitted={onBankSubmitted}
                {...tab}
            />
            <AgentPasswordPanel
                record={record}
                brandId={brandId}
                agentId={agentId}
                onSubmitted={onSubmitted}
                onClickClose={onClickClose}
                {...tab}
            />
        </Tabs>
    )
}
