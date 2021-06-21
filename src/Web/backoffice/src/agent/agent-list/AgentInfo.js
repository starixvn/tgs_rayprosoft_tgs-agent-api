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
    useClassName,
    Alert
} from "bumbag"
import { FieldValues } from "../../layout/Grid"
import {
    Trans,
    FormattedDateTime,
    FormattedNumber,
    useTrans,
    FormattedNumberDecimal
} from "../../i18n"
import AlertDialog from "../../dialog/Alert"
import { SubmitCancel } from "../../button/ActionButtons"
import { Tab as _Tab, TabPanel } from "../../tab"
import {
    FormProvider,
    useForm,
    useFormContext,
    useWatch
} from "react-hook-form"
import { Input, InputField } from "../../input/Input"
import { useAgent, useUpdateAgent } from "./client"
import { AgentStatusLabel, AgentStatusSelect } from "../AgentStatus"
import { AgentLevelLabel, AgentLevelSelect, isAgentHead } from "../AgentLevel"
import {
    AgentTypeLabel,
    AgentTypeSelect,
    isAgentTypeCash,
    isAgentTypeCredit
} from "../AgentType"
import clone from "clone-deep"
import Select from "../../select/Select"
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
import AgentLevel from "../AgentLevel"

const Key = "AgentInfo"

export const AgentInfoKey = Key

export const AgentInfoTab = ({ ...props }) => {
    return (
        <_Tab tabId={Key} {...props}>
            <Trans zh="代理信息" en="Agent Info" />
        </_Tab>
    )
}

export const AgentInfoPanel = ({ readonly = false, ...props }) =>
    readonly ? <ViewPanel {...props} /> : <EditPanel {...props} />

const EditPanel = ({ record, onSubmitted, onClickClose, ...props }) => {
    let formMethods = useForm({
        defaultValues: clone(record)
    })
    let { control } = formMethods

    let agentId = record?.id
    let [formData, setSubmit] = useState()
    let { isLoading, result, error, mutate, deleteCache } = useUpdateAgent(
        agentId,
        formData
    )

    useEffect(() => {
        if (result && !error) {
            reset()
            onSubmitted && onSubmitted()
        }
    }, [result])

    let agentUsernamePlaceholder = useTrans({
        zh: "代理用户名",
        en: "Agent Username"
    })

    const ParentUsernameFieldName = "parentUsername"
    let currentParentUsername = record.parentUsername
    let parentUsername = useWatch({
        control,
        name: ParentUsernameFieldName
    })

    let willChangeParent =
        currentParentUsername &&
        parentUsername &&
        currentParentUsername != parentUsername

    const LevelTypeFieldName = "levelType"
    let levelType = useWatch({
        control,
        name: LevelTypeFieldName
    })
    let changeLevelType = levelType != record.levelType
    let displayParentUsername =
        changeLevelType && levelType == AgentLevel.Chief ? "none" : "block"

    const onSubmit = (data) => {
        setSubmit({
            ...record,
            updateType: "info",
            ...data
        })
    }

    const reset = () => {
        deleteCache()
        mutate(undefined, false)
        setSubmit(undefined)
    }

    let agentHead = isAgentHead(record?.levelType)

    const AgentTypeField = "type"
    let agentType = useWatch({
        control,
        name: AgentTypeField
    })
    return (
        <TabPanel tabId={Key} {...props}>
            <FormProvider {...formMethods}>
                <form onSubmit={formMethods.handleSubmit(onSubmit)}>
                    <Stack>
                        <FieldValues>
                            <UsernameBox value={record?.username} />

                            <RealNameBox>
                                <Input name="realName" />
                            </RealNameBox>

                            <AgentLevelBox>
                                <AgentLevelSelect
                                    name={LevelTypeFieldName}
                                    disabled={agentHead}
                                />
                            </AgentLevelBox>

                            {!agentHead && (
                                <>
                                    <ParentAgentUsername
                                        display={displayParentUsername}
                                    >
                                        <Set
                                            orientation="vertical"
                                            isFilled
                                            display={displayParentUsername}
                                        >
                                            <Input
                                                name={ParentUsernameFieldName}
                                                placeholder={
                                                    agentUsernamePlaceholder
                                                }
                                                required
                                                disabled={
                                                    changeLevelType &&
                                                    levelType ==
                                                        AgentLevel.Chief
                                                }
                                            />
                                            {willChangeParent && (
                                                <ChangeParentWarning />
                                            )}
                                        </Set>
                                    </ParentAgentUsername>
                                </>
                            )}

                            <RegistrationBox value={record?.registeredOn} />

                            <BalanceBox value={record?.balance} />

                            <AgentStatusBox>
                                <AgentStatusSelect
                                    name="status"
                                    placeholder={false}
                                    required
                                />
                            </AgentStatusBox>

                            <AgentTypeBox>
                                <Box alignY="center">
                                    {!agentHead && (
                                        <>
                                            <AgentTypeSelect
                                                name={AgentTypeField}
                                                value={record?.type}
                                                display="none"
                                            />
                                            <AgentTypeSelect
                                                value={record?.type}
                                                disabled
                                            />
                                        </>
                                    )}
                                    {agentHead && (
                                        <AgentTypeSelect
                                            name={AgentTypeField}
                                        />
                                    )}
                                </Box>
                            </AgentTypeBox>

                            {!willChangeParent && (
                                <DividendBox type={agentType}>
                                    <AgentDividendPercentageInputField />
                                </DividendBox>
                            )}

                            {!willChangeParent && (
                                <RebateBox
                                    label={<AgentLiveDealerRebateText />}
                                    type={agentType}
                                >
                                    <AgentLiveDealerRebatePercentageInputField />
                                </RebateBox>
                            )}

                            {!willChangeParent && (
                                <RebateBox
                                    label={<AgentEgameRebateText />}
                                    type={agentType}
                                >
                                    <AgentEgameRebatePercentageInputField />
                                </RebateBox>
                            )}

                            {!willChangeParent && (
                                <RebateBox
                                    label={<AgentCardGameRebateText />}
                                    type={agentType}
                                >
                                    <AgentCardGameRebatePercentageInputField />
                                </RebateBox>
                            )}

                            {!willChangeParent && (
                                <RebateBox
                                    label={<AgentLotteryRebateText />}
                                    type={agentType}
                                >
                                    <AgentLotteryRebatePercentageInputField />
                                </RebateBox>
                            )}

                            {!willChangeParent && (
                                <RebateBox
                                    label={<AgentSportsRebateText />}
                                    type={agentType}
                                >
                                    <AgentSportsRebatePercentageInputField />
                                </RebateBox>
                            )}

                            {!willChangeParent && (
                                <RebateBox
                                    label={<AgentVirtualSportsRebateText />}
                                    type={agentType}
                                >
                                    <AgentVirtualSportsRebatePercentageInputField />
                                </RebateBox>
                            )}
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

const ChangeParentWarning = () => {
    let warning = useTrans({
        zh: "更改上級需重新配置",
        en: "Change the upper level agent required to re-config."
    })
    return <Alert variant="tint" title={warning} type="warning" />
}

const ViewPanel = ({ record, ...props }) => {
    return (
        <TabPanel tabId={Key} {...props}>
            <FieldValues>
                <UsernameBox value={record?.username} />

                <RealNameBox value={record?.realName} />

                <ParentAgentUsername value={record?.parentUsername} />

                <RegistrationBox value={record?.registeredOn} />

                <BalanceBox value={record?.balance} />

                <AgentStatusBox value={record?.status} />

                <LevelTypeBox value={record?.levelType} />

                <AgentTypeBox value={record?.type} />

                <DividendBox
                    value={record?.dividendPercentage}
                    type={record?.type}
                />

                <RebateBox
                    label={<AgentLiveDealerRebateText />}
                    value={record?.liveDealerRebatePercentage}
                    type={record?.type}
                />

                <RebateBox
                    label={<AgentEgameRebateText />}
                    value={record?.slotRebatePercentage}
                    type={record?.type}
                />

                <RebateBox
                    label={<AgentCardGameRebateText />}
                    value={record?.cardGameRebatePercentage}
                    type={record?.type}
                />

                <RebateBox
                    label={<AgentLotteryRebateText />}
                    value={record?.lotteryRebatePercentage}
                    type={record?.type}
                />

                <RebateBox
                    label={<AgentSportsRebateText />}
                    value={record?.sportsRebatePercentage}
                    type={record?.type}
                />

                <RebateBox
                    label={<AgentVirtualSportsRebateText />}
                    value={record?.virtualSportsRebatePercentage}
                    type={record?.type}
                />
            </FieldValues>
        </TabPanel>
    )
}

const UsernameBox = ({ value = "", children }) => {
    return (
        <>
            <Box alignY="center">
                <Trans zh="代理用户名" en="Agent Username" />
            </Box>
            <Text>{value}</Text>
        </>
    )
}

const RealNameBox = ({ value = "", children }) => {
    return (
        <>
            <Box alignY="center">
                <Trans zh="真实姓名" en="Real name" />
            </Box>
            {children ? children : <Text>{value}</Text>}
        </>
    )
}

const AgentLevelBox = ({ value = "", children }) => {
    return (
        <>
            <Box alignY="center">
                <Trans zh="代理种类" en="Level Type" />
            </Box>
            {children ? children : <AgentLevelLabel value={value} />}
        </>
    )
}

const ParentAgentUsername = ({ value = "", children, display }) => {
    return (
        <>
            <Box alignY="center" display={display}>
                <Trans zh="上级代理" en="Upper level agent" />
            </Box>

            {children ? children : <Text display={display}>{value}</Text>}
        </>
    )
}

const RegistrationBox = ({ value }) => {
    return (
        <>
            <Trans zh="注册时间" en="Registration" />
            <Text>
                <FormattedDateTime value={value} />
            </Text>
        </>
    )
}

const BalanceBox = ({ value = "" }) => {
    return (
        <>
            <Trans zh="余额" en="Balance" />
            <Text>
                <FormattedNumberDecimal value={value} />
            </Text>
        </>
    )
}

const AgentStatusBox = ({ value = "", children }) => {
    return (
        <>
            <Trans zh="状态" en="Status" />

            {children ? (
                children
            ) : (
                <Text>
                    <AgentStatusLabel value={value} />
                </Text>
            )}
        </>
    )
}

const LevelTypeBox = ({ value = "", children }) => {
    return (
        <>
            <Trans zh="代理种类" en="Level Type" />

            {children ? (
                children
            ) : (
                <Text>
                    <AgentLevelLabel value={value} />
                </Text>
            )}
        </>
    )
}

const AgentTypeBox = ({ value, children }) => {
    return (
        <>
            <Trans zh="代理类型" en="Agent Type" />
            {children ? (
                children
            ) : (
                <Text>
                    <AgentTypeLabel value={value} />
                </Text>
            )}
        </>
    )
}

const DividendBox = ({ value, type, children }) => {
    let isCash = isAgentTypeCash(type)
    if (!isCash) {
        return null
    }

    return (
        <>
            <Trans zh="分红比例" en="Profit Sharing Percentage" />
            {children ? (
                children
            ) : (
                <Text>
                    <FormattedNumberDecimal value={value} postfix="%" />
                </Text>
            )}
        </>
    )
}

const RebateBox = ({ label, value, type, children }) => {
    let isCredit = isAgentTypeCredit(type)
    if (!isCredit) {
        return null
    }

    return (
        <>
            {label}
            {children ? (
                children
            ) : (
                <Text>
                    <FormattedNumberDecimal value={value} postfix="%" />
                </Text>
            )}
        </>
    )
}
