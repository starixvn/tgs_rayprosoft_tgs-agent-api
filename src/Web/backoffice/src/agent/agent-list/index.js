import React, { useState, useEffect, useMemo } from "react"
import { css, Global } from "@emotion/react"
import { useForm, FormProvider, useFormContext } from "react-hook-form"
import {
    Heading,
    Stack,
    Box,
    Button,
    Set,
    useModalState,
    Flex,
    Level
} from "bumbag"
import PageContent from "../../page/Content"
import sequenceColumn from "../../table/column/sequence"
import dateTimeColumn from "../../table/column/dateTime"
import { useAgentList } from "./client"
import { useTable, Table, pageCount, toApiParams } from "../../table"
import { AgentStatusLabel } from "../AgentStatus"
import {
    useIsDisplayMenuByRoute,
    useHasSubMenuByRoute,
    useUserBrandId
} from "../../user/context"
import { Code } from "./menu"
import { AgentStatusSelect } from "../AgentStatus"
import { VipLevelSelect } from "../../vip-levels/VipLevelSelect"
import { ViewButton, EditButton, ConfigButton, AddAgentButton } from "./button"
import { FormattedNumberDecimal, Trans, useTrans } from "../../i18n"
import SearchButton from "../../button/SearchButton"
import equals from "fast-deep-equal"
import { InputField } from "../../input/Input"
import BrandBar from "../../report/BrandBar"
import AlertDialog from "../../dialog/Alert"
import textColumn from "../../table/column/textColumn"
import numberColumn, {
    numberDecimalColumn,
    numberDecimalPercentageColumn
} from "../../table/column/numberColumn"
import { AgentLevelLabel, AgentLevelSelect } from "../AgentLevel"
import { AgentTypeLabel, AgentTypeSelect } from "../AgentType"
import {
    AgentConfigStatusLabel,
    isAgentConfigRequired,
    isAgentConfigDone
} from "../AgentConfigStatus"
import AgentDividendPercentage from "../AgentDividendPercentage"
import AgentLiveDealerRebatePercentage from "../AgentLiveDealerRebatePercentage"
import AgentEgameRebatePercentage from "../AgentEgameRebatePercentage"
import AgentCardGameRebatePercentage from "../AgentCardGameRebatePercentage"
import AgentLotteryRebatePercentage from "../AgentLotteryRebatePercentage"
import AgentSportsRebatePercentage from "../AgentSportsRebatePercentage"
import AgentVirtualSportsRebatePercentage from "../AgentVirtualSportsRebatePercentage"

export { Code }

export const Content = () => {
    let isDisplay = useIsDisplayMenuByRoute()
    if (!isDisplay) {
        return null
    }

    return <Body />
}

export const CodeContentPair = [Code, Content]

const Body = () => {
    let userBrandId = useUserBrandId()
    let [brandId, setBrandId] = useState(userBrandId)

    return (
        <Set orientation="vertical" isFilled>
            <BrandBar
                onBrandChange={(value) => {
                    setBrandId(value)
                }}
            />
            {brandId && <BodyContent brandId={brandId} />}
        </Set>
    )
}

const BodyContent = ({ brandId = "" }) => {
    let { state, setState, setPageIndex, setSortOrder } = useTable({
        status: "",
        levelType: "",
        type: "",
        username: ""
    })

    useEffect(() => {
        setState({ pageIndex: 0, brandId })
    }, [brandId])

    let { pageIndex, pageSize, status, levelType, type, username } = state
    let { data = {}, isLoading, mutate, error, deleteCache } = useAgentList(
        brandId
            ? {
                  ...toApiParams(state),
                  sortby: "username",
                  brandId
              }
            : undefined
    )

    let columns = useColumns({ mutate, items: data?.items, brandId })

    return (
        <PageContent
            title={<Trans zh="代理列表" en="Agents" />}
            actionBar={
                <Flex justifyContent="space-between">
                    <SearchBar
                        brandId={brandId}
                        search={(data) => {
                            let newState = {
                                pageIndex: 0,
                                status: data.status,
                                levelType: data.levelType,
                                type: data.type,
                                username: data.username
                            }
                            if (
                                equals(newState, {
                                    pageIndex,
                                    status,
                                    levelType,
                                    type,
                                    username
                                })
                            ) {
                                mutate()
                            }
                            setState({
                                pageIndex: 0,
                                ...data
                            })
                        }}
                        isLoading={isLoading}
                    />
                    <AddAgentButton brandId={brandId} onSubmitted={mutate} />
                </Flex>
            }
        >
            <Table
                controlledState={state}
                columns={columns}
                data={data?.items}
                pageCount={pageCount(pageSize, data?.totalCount)}
                setPageIndex={setPageIndex}
                setSortOrder={setSortOrder}
                isLoading={isLoading}
            />
            {error && (
                <AlertDialog
                    {...error}
                    onClickClose={() => {
                        deleteCache()
                        mutate({ data: undefined }, false)
                    }}
                />
            )}
        </PageContent>
    )
}

const SearchBar = ({ brandId, search, isLoading, ...props }) => {
    let usernameInputPlaceholder = useSearchInputPlaceHolder()
    let formMethods = useForm()

    const onSubmit = (data) => search(data)

    return (
        <FormProvider {...formMethods}>
            <form onSubmit={formMethods.handleSubmit(onSubmit)}>
                <Set>
                    <AgentStatusSelect name="status" />
                    <AgentLevelSelect name="levelType" />
                    <AgentTypeSelect name="type" />
                    <InputField
                        name="username"
                        showEmptyFieldValueAddonAfter
                        placeholder={usernameInputPlaceholder}
                    />
                    <SearchButton isLoading={isLoading} disabled={isLoading} />
                </Set>
            </form>
        </FormProvider>
    )
}

const useSearchInputPlaceHolder = () =>
    useTrans({ zh: "用户名输入框", en: "Input user name" })

const useColumns = ({ mutate, items = [], brandId } = {}) => {
    let hasAnyMenu = useHasSubMenuByRoute()

    return useMemo(() => {
        let columns = [
            sequenceColumn(),
            textColumn({
                accessor: "username",
                Header: <Trans zh="用户名" en="User name" />,
                disableSortBy: true
            }),
            textColumn({
                accessor: "levelType",
                Header: <Trans zh="代理种类" en="Level Type" />,
                disableSortBy: true,
                Cell: ({ value }) => <AgentLevelLabel value={value} />
            }),
            textColumn({
                accessor: "type",
                Header: <Trans zh="代理类型" en="Agent Type" />,
                disableSortBy: true,
                Cell: ({ value }) => <AgentTypeLabel value={value} />
            }),
            textColumn({
                accessor: "status",
                Header: <Trans zh="状态" en="Status" />,
                disableSortBy: true,
                Cell: ({ value }) => <AgentStatusLabel value={value} />
            }),
            textColumn({
                id: AgentDividendPercentage.Name,
                accessor: (row) => row,
                Header: <Trans zh="分红比例" en="Dividend Percentage" />,
                disableSortBy: true,
                Cell: ({ value }) => {
                    let configRequired = isAgentConfigRequired(
                        value?.agentStatus
                    )
                    if (configRequired) {
                        return "-"
                    }

                    return (
                        <FormattedNumberDecimal
                            value={value?.dividendPercentage}
                            postfix="%"
                        />
                    )
                }
            }),
            textColumn({
                id: AgentLiveDealerRebatePercentage.Name,
                accessor: (row) => row,
                Header: <Trans zh="真人返佣%" en="Live Dealer Rebate %" />,
                disableSortBy: true,
                Cell: ({ value }) => {
                    let configRequired = isAgentConfigRequired(
                        value?.agentStatus
                    )
                    if (configRequired) {
                        return "-"
                    }

                    return (
                        <FormattedNumberDecimal
                            value={value?.liveDealerRebatePercentage}
                            postfix="%"
                        />
                    )
                }
            }),
            textColumn({
                id: AgentEgameRebatePercentage.Name,
                accessor: (row) => row,
                Header: <Trans zh="电子返佣%" en="EGame Rebate %" />,
                disableSortBy: true,
                Cell: ({ value }) => {
                    let configRequired = isAgentConfigRequired(
                        value?.agentStatus
                    )
                    if (configRequired) {
                        return "-"
                    }

                    return (
                        <FormattedNumberDecimal
                            value={value?.slotRebatePercentage}
                            postfix="%"
                        />
                    )
                }
            }),
            textColumn({
                id: AgentCardGameRebatePercentage.Name,
                accessor: (row) => row,
                Header: <Trans zh="棋牌返佣%" en="Card Game Rebate %" />,
                disableSortBy: true,
                Cell: ({ value }) => {
                    let configRequired = isAgentConfigRequired(
                        value?.agentStatus
                    )
                    if (configRequired) {
                        return "-"
                    }

                    return (
                        <FormattedNumberDecimal
                            value={value?.cardGameRebatePercentage}
                            postfix="%"
                        />
                    )
                }
            }),
            textColumn({
                id: AgentLotteryRebatePercentage.Name,
                accessor: (row) => row,
                Header: <Trans zh="彩票返佣%" en="Lottery Rebate %" />,
                disableSortBy: true,
                Cell: ({ value }) => {
                    let configRequired = isAgentConfigRequired(
                        value?.agentStatus
                    )
                    if (configRequired) {
                        return "-"
                    }

                    return (
                        <FormattedNumberDecimal
                            value={value?.lotteryRebatePercentage}
                            postfix="%"
                        />
                    )
                }
            }),
            textColumn({
                id: AgentSportsRebatePercentage.Name,
                accessor: (row) => row,
                Header: <Trans zh="体育返佣%" en="Sports Rebate %" />,
                disableSortBy: true,
                Cell: ({ value }) => {
                    let configRequired = isAgentConfigRequired(
                        value?.agentStatus
                    )
                    if (configRequired) {
                        return "-"
                    }

                    return (
                        <FormattedNumberDecimal
                            value={value?.sportsRebatePercentage}
                            postfix="%"
                        />
                    )
                }
            }),
            textColumn({
                id: AgentVirtualSportsRebatePercentage.Name,
                accessor: (row) => row,
                Header: (
                    <Trans zh="虚拟体育返佣%" en="Virtual Sports Rebate %" />
                ),
                disableSortBy: true,
                Cell: ({ value }) => {
                    let configRequired = isAgentConfigRequired(
                        value?.agentStatus
                    )
                    if (configRequired) {
                        return "-"
                    }

                    return (
                        <FormattedNumberDecimal
                            value={value?.virtualSportsRebatePercentage}
                            postfix="%"
                        />
                    )
                }
            }),
            textColumn({
                accessor: "agentStatus",
                Header: <Trans zh="配置状态" en="Config Status" />,
                disableSortBy: true,
                Cell: ({ value }) => {
                    let configRequired = isAgentConfigRequired(value)

                    return (
                        <AgentConfigStatusLabel
                            value={value}
                            fontWeight={configRequired ? "bold" : undefined}
                        />
                    )
                }
            }),
            numberColumn({
                accessor: "teamNumber",
                Header: <Trans zh="团队人数" en="Team Number" />,
                disableSortBy: true
            })
        ]

        if (hasAnyMenu) {
            columns.push({
                id: "actions",
                disableSortBy: true,
                Header: <Trans zh="操作" en="Actions" />,
                headCell: {
                    textAlign: "center"
                },
                cell: {
                    textAlign: "center"
                },
                accessor: (row) => {
                    return row
                },
                Cell: ({ value }) => {
                    let { agentStatus } = value
                    let configDone = isAgentConfigDone(agentStatus)
                    let configRequired = isAgentConfigRequired(agentStatus)
                    return (
                        <Set alignX="center">
                            {configDone && (
                                <>
                                    <ViewButton
                                        record={value}
                                        brandId={brandId}
                                    />
                                    <EditButton
                                        brandId={brandId}
                                        record={value}
                                        onSubmitted={mutate}
                                    />
                                </>
                            )}
                            {configRequired && (
                                <ConfigButton
                                    palette="warning"
                                    variant="outlined"
                                    brandId={brandId}
                                    agentId={value?.id}
                                    onSubmitted={mutate}
                                />
                            )}
                        </Set>
                    )
                }
            })
        }

        return columns
    }, [JSON.stringify(items)])
}
