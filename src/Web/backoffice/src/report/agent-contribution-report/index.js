import React, { useState, useEffect, useMemo } from "react"
import { css, Global } from "@emotion/react"
import { useForm, FormProvider, useFormContext } from "react-hook-form"
import {
    Box,
    Breadcrumb,
    Set,
    useModalState,
    Text,
    Link,
    FieldStack,
    Icon
} from "bumbag"
import PageContent from "../../page/Content"
import {
    useIsDisplayMenuByRoute,
    useHasSubMenuByRoute,
    useUserBrandId
} from "../../user/context"
import { useTable, Table, pageCount, toApiParams } from "../../table"
import { FormattedNumber, Trans, useTrans } from "../../i18n"
import SearchButton from "../../button/SearchButton"
import equals from "fast-deep-equal"
import { InputField } from "../../input/Input"
import { Code } from "./menu"
import BrandBar from "../BrandBar"
import { DateInputRange } from "../../date/DateTimeRangeInput"
import { yesterday } from "../../date/range"
import dateTimeColumn from "../../table/column/dateTime"
import subDays from "date-fns/subDays"
import { startOfNextDay } from "../../date/startOfNextDay"
import numberColumn, {
    numberDecimalColumn
} from "../../table/column/numberColumn"
import startOfToday from "date-fns/startOfToday"
import AlertDialog from "../../dialog/Alert"
import textColumn from "../../table/column/textColumn"
import { useAgentContributionList } from "./client"
import { AgentTypeSelect } from "../../agent/AgentType"
import Checkbox from "../../checkbox"
import headOf from "../../list/head"
import lastOf, { isLastIndex } from "../../list/last"
import { faChevronRight } from "@fortawesome/free-solid-svg-icons"

export { Code }

export const Content = () => {
    let isDisplay = useIsDisplayMenuByRoute()
    if (!isDisplay) {
        return null
    }

    return <Body />
}

export const CodeContentPair = [Code, Content]

export const Body = () => {
    let userBrandId = useUserBrandId()
    let [brandId, setBrandId] = useState(userBrandId)

    return (
        <Set orientation="vertical" isFilled>
            <BrandBar
                onBrandChange={(value) => {
                    setBrandId(value)
                }}
            />
            {brandId && <ReportContent brandId={brandId} />}
        </Set>
    )
}

const ReportContent = ({ brandId }) => {
    let [usernames, setUsernames] = useState([])
    let username = lastOf(usernames)
    let agentUsername = headOf(usernames)

    let dateRange = yesterday()
    let QueryMaxRangeDays = 30
    let { state, setState, setPageIndex, setSortOrder } = useTable({
        brandId,
        pageSize: QueryMaxRangeDays,
        agentType: undefined,
        includeAgents: true,
        from: dateRange.start,
        to: dateRange.end
    })

    useEffect(() => {
        setState({ pageIndex: 0, brandId })
    }, [brandId])

    useEffect(() => {
        setPageIndex(0)
    }, [JSON.stringify(usernames)])

    let { pageIndex, pageSize, from, to } = state
    let {
        isLoading,
        data = {},
        error,
        mutate,
        deleteCache
    } = useAgentContributionList(
        username
            ? {
                  ...toApiParams(state),
                  username,
                  sortby: "username",
                  from,
                  to: startOfNextDay(to)
              }
            : undefined
    )

    let DataRetentionDays = 180
    let minDate = subDays(dateRange.start, DataRetentionDays)

    let columns = useColumns({
        brandId,
        usernames,
        setUsernames,
        start: from,
        end: to,
        items: data?.items
    })
    let maxDate = startOfToday()

    return (
        <PageContent
            actionBar={
                <SearchBar
                    brandId={brandId}
                    agentUsername={agentUsername}
                    agentType={state?.agentType}
                    includeAgents={state?.includeAgents}
                    start={from}
                    end={to}
                    minDate={minDate}
                    maxDate={maxDate}
                    maxRangeDays={QueryMaxRangeDays}
                    isLoading={isLoading}
                    search={({
                        agentUsername,
                        agentType,
                        includeAgents,
                        start,
                        end
                    }) => {
                        let newState = {
                            pageIndex: 0,
                            agentType,
                            includeAgents,
                            from: start,
                            to: end
                        }

                        if (
                            equals(newState, {
                                pageIndex,
                                agentType: state?.agentType,
                                includeAgents: state?.includeAgents,
                                from: state?.from,
                                to: state?.to
                            })
                        ) {
                            mutate()
                        }

                        setUsernames([agentUsername])

                        setState({
                            pageIndex: 0,
                            agentType,
                            includeAgents,
                            from: start,
                            to: end
                        })
                    }}
                />
            }
            title={<Trans zh="代理贡献报表" en="Agent Contribution Report" />}
        >
            <FieldStack>
                <AgentBreadcrumb
                    usernames={usernames}
                    setUsernames={setUsernames}
                />
                {username && (
                    <Table
                        controlledState={state}
                        columns={columns}
                        data={data?.items}
                        pageCount={pageCount(pageSize, data?.totalCount)}
                        setPageIndex={setPageIndex}
                        setSortOrder={setSortOrder}
                        isLoading={isLoading}
                        foot={data?.items?.length > 0}
                    />
                )}
            </FieldStack>
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

const AgentBreadcrumb = ({ usernames = [], setUsernames }) => {
    if (usernames.length === 0) {
        return null
    }

    return (
        <Breadcrumb
            marginLeft="1rem"
            separator={
                <Icon icon={faChevronRight} type="font-awesome" color="gray" />
            }
        >
            {usernames.map((username, index, list) => {
                let path = list.slice(0, index + 1)

                let isCurrent = isLastIndex(index, list)
                return (
                    <Breadcrumb.Item isCurrent={isCurrent}>
                        <Breadcrumb.Link
                            onClick={(event) => {
                                if (!isCurrent) {
                                    setUsernames(path)
                                }
                            }}
                        >
                            {username}
                        </Breadcrumb.Link>
                    </Breadcrumb.Item>
                )
            })}
        </Breadcrumb>
    )
}

const SearchBar = ({
    brandId,
    agentUsername,
    agentType,
    includeAgents,
    start,
    end,
    minDate,
    maxDate,
    maxRangeDays,
    search,
    isLoading,
    ...props
}) => {
    let formMethods = useForm({
        defaultValues: {
            agentUsername,
            agentType,
            includeAgents,
            start,
            end
        }
    })

    let usernamePlaceholder = useTrans({
        zh: "代理用户名",
        en: "Agent username"
    })

    let includeAgentCheckboxLabel = useTrans({
        zh: "包含代理资讯",
        en: "Include Agent Info"
    })

    const onSubmit = (data) => search?.(data)

    return (
        <FormProvider {...formMethods}>
            <form onSubmit={formMethods.handleSubmit(onSubmit)}>
                <Set>
                    <InputField
                        name="agentUsername"
                        showEmptyFieldValueAddonAfter
                        placeholder={usernamePlaceholder}
                        requiredMessage=""
                        required
                        autoFocus
                    />
                    <AgentTypeSelect name="agentType" />
                    <DateInputRange
                        minDate={minDate}
                        maxDate={maxDate}
                        maxRangeDays={maxRangeDays}
                        required
                    />
                    <Checkbox
                        name="includeAgents"
                        label={includeAgentCheckboxLabel}
                    />
                    <SearchButton isLoading={isLoading} disabled={isLoading} />
                </Set>
            </form>
        </FormProvider>
    )
}

const useColumns = ({
    brandId,
    start,
    end,
    usernames = [],
    setUsernames,
    items = []
}) => {
    return useMemo(
        () => [
            textColumn({
                id: "username",
                accessor: (row) => row,
                Header: <Trans zh="用户名" en="Username" />,
                Cell: ({ value }) => {
                    //TODO: if type is Agent, setUsernames?
                    return (
                        <Link
                            onClick={(event) => {
                                let path = [...usernames, value?.username]
                                console.log("path", path)
                                setUsernames(path)
                            }}
                        >
                            {value?.username}
                        </Link>
                    )
                },
                Footer: () => <Trans zh="总计" en="Total" />,
                footerCellProps: {
                    textAlign: "center"
                },
                disableSortBy: true
            }),
            textColumn({
                id: "accountType",
                accessor: (row) => row,
                Header: <Trans zh="账户类型" en="Account Type" />,
                Cell: ({ value }) => (
                    <Text.Block>
                        <Trans zh="玩家" en="Player" /> or
                        <Trans zh="代理" en="Agent" />
                    </Text.Block>
                ),
                Footer: () => <Box />,
                disableSortBy: true
            }),
            numberDecimalColumn({
                accessor: "depositAmount",
                sumColumn: "depositAmount",
                Header: <Trans zh="存款金额" en="Deposit Amount" />,
                disableSortBy: true
            }),
            numberDecimalColumn({
                accessor: "withdrawalAmount",
                sumColumn: "withdrawalAmount",
                Header: <Trans zh="提款金额" en="Withdrawal Amount" />,
                disableSortBy: true
            }),
            numberDecimalColumn({
                accessor: "bonusAmount",
                sumColumn: "bonusAmount",
                Header: <Trans zh="红利金额" en="Bonus Amount" />,
                disableSortBy: true
            }),
            numberDecimalColumn({
                accessor: "adjustmentAmount",
                sumColumn: "adjustmentAmount",
                Header: <Trans zh="调账" en="Adjustment Amount" />,
                disableSortBy: true
            }),
            numberDecimalColumn({
                accessor: "betAmount",
                sumColumn: "betAmount",
                Header: <Trans zh="投注金额" en="Bet Amount" />,
                disableSortBy: true
            }),
            numberDecimalColumn({
                accessor: "validBetAmount",
                sumColumn: "validBetAmount",
                Header: <Trans zh="有效投注" en="Valid Bet Amount" />,
                disableSortBy: true
            }),
            numberDecimalColumn({
                accessor: "winLossAmount",
                sumColumn: "winLossAmount",
                Header: <Trans zh="输赢金额" en="Win Loss Amount" />,
                disableSortBy: true
            }),
            numberDecimalColumn({
                accessor: "netProfit",
                sumColumn: "netProfit",
                Header: <Trans zh="净盈利" en="Net Profit" />,
                disableSortBy: true
            })
        ],
        [JSON.stringify(items), JSON.stringify(usernames)]
    )
}
