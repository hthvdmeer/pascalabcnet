// Copyright (c) Ivan Bondarev, Stanislav Mikhalkovich (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using System.Collections.Generic;

namespace PascalABCCompiler.SemanticTree
{

	//Kind of object.
	//basic - a basic object not defined in the program, for example the addition of two integers.
	//common - an ordinary type, method, etc., defined by the user.
	//compiled - a type, method, or other node defined by the user.
	public enum node_kind {basic,common,compiled,indefinite};

	//Access level of the class. Though why am I writing this? The name already makes it clear.
	public enum type_access_level {tal_public,tal_internal};

	//How shall we represent reference and value types?
	//public enum reference_or_value_type {reference_type,value_type};

	//Location of an element - inside a function, a class, or a namespace.
	public enum node_location_kind {in_function_location,in_class_location,in_namespace_location, in_block_location, indefinite};

	//Access level for class members.
    public enum field_access_level { fal_private, fal_internal, fal_protected, fal_public };
	
	//Ordinary, static, or virtual class member.
	public enum polymorphic_state {ps_static,ps_common,ps_virtual,ps_virtual_abstract};

    public enum type_special_kind { none_kind, not_set_kind, array_kind, enum_kind, typed_file, binary_file, short_string, array_wrapper, record, set_type, base_set_type, diap_type, text_file };
	
    public enum attribute_qualifier_kind {none_kind, return_kind, assembly_kind, param_kind, type_kind, field_kind, event_kind, property_kind, method_kind}
    //Parameter passing mode - by reference or by value.
    public enum parameter_type { value, var, cnst };

    //Type of a basic function.
    public enum basic_function_type
    {
        none,
        iadd, isub, imul, idiv, imod, igr, ism, igreq, ismeq, ieq, inoteq, ishl, ishr, ior, inot, ixor, iand, iunmin, iinc, idec, isinc, isdec, iassign,  //signed integer (4 byte)
        uiadd, uisub, uimul, uidiv, uimod, uigr, uism, uigreq, uismeq, uieq, uinoteq, uishl, uishr, uior, uinot, uixor, uiand, uiunmin, uiinc, uidec, uisinc, uisdec, uiassign, //unsigned integer (4 byte)
        badd, bsub, bmul, bdiv, bmod, bgr, bsm, bgreq, bsmeq, beq, bnoteq, bshl, bshr, bor, bnot, bxor, band, bunmin, binc, bdec, bsinc, bsdec, bassign, //unsigned byte
        sbadd, sbsub, sbmul, sbdiv, sbmod, sbgr, sbsm, sbgreq, sbsmeq, sbeq, sbnoteq, sbshl, sbshr, sbor, sbnot, sbxor, sband, sbunmin, sbinc, sbdec, sbsinc, sbsdec, sbassign, //signed byte
        sadd, ssub, smul, sdiv, smod, sgr, ssm, sgreq, ssmeq, seq, snoteq, sshl, sshr, sor, snot, sxor, sand, sunmin, sinc, sdec, ssinc, ssdec, sassign, //short (2-byte)
        usadd, ussub, usmul, usdiv, usmod, usgr, ussm, usgreq, ussmeq, useq, usnoteq, usshl, usshr, usor, usnot, usxor, usand, usunmin, usinc, usdec, ussinc, ussdec, usassign, //unsigned short (2 byte)
        ladd, lsub, lmul, ldiv, lmod, lgr, lsm, lgreq, lsmeq, leq, lnoteq, lshl, lshr, lor, lnot, lxor, land, lunmin, linc, ldec, lsinc, lsdec, lassign, //long (8 byte)
        uladd, ulsub, ulmul, uldiv, ulmod, ulgr, ulsm, ulgreq, ulsmeq, uleq, ulnoteq, ulshl, ulshr, ulor, ulnot, ulxor, uland, ulunmin, ulinc, uldec, ulsinc, ulsdec, ulassign, //unsigned long (8 byte)
        fadd, fsub, fmul, fdiv, fgr, fsm, fgreq, fsmeq, feq, fnoteq, funmin, fassign,//float
        dadd, dsub, dmul, ddiv, dgr, dsm, dgreq, dsmeq, deq, dnoteq, dunmin, dassign,//double
        boolgr, boolsm, boolgreq, boolsmeq, booleq, boolnoteq, boolor, boolnot, boolxor, booland, boolassign,  //boolean
        chargr, charsm, chargreq, charsmeq, chareq, charnoteq, cinc, cdec, csinc, csdec, charassign,  //char
        chartous, chartoi, chartoui, chartol, chartoul, chartof, chartod, chartob, chartosb, chartos,
        btos, btous, btoi, btoui, btol, btoul, btof, btod, btosb, btochar, //byte to ...
        sbtos, sbtoi, sbtol, sbtof, sbtod, sbtob, sbtous, sbtoui, sbtoul, sbtochar,//signed byte to short, int, long, float, double
        stoi, stol, stof, stod, stob, stosb, stous, stoui, stoul, stochar, //short to ...
        ustoi, ustoui, ustol, ustoul, ustof, ustod, ustob, ustosb, ustos, ustochar,  //unsigned short to ...
        itol, itof, itod, itob, itosb, itos, itous, itoui, itoul, itochar, itobool,    //integer to ...
        uitol, uitoul, uitob, uitosb, uitos, uitous, uitoi, uitof, uitod, uitochar,    //uint to ...
        ltof, ltod, ltob, ltosb, ltos, ltous, ltoi, ltoui, ltoul, ltochar,     //long to ...
        ultob, ultosb, ultos, ultous, ultoi, ultoui, ultol, ultochar, ultof, ultod, //ulong to ...
        ftod, ftob, ftosb, ftos, ftous, ftoi, ftoui, ftol, ftoul, ftochar, //float to ...
        dtob, dtosb, dtos, dtous, dtoi, dtoui, dtol, dtoul, dtof, dtochar,
        objassign, objeq, objnoteq, //assignment and reference equality of objects.
                                    //write,writei,writed,writec,writeb,read,readi,readd,readc,readb,expd,absd,absi //temporary functions (Needed only in the early debugging stage. Must be removed later.)
        objtoobj, boolinc, booldec, boolsinc, boolsdec, booltoi, enumgr, enumgreq, enumsm, enumsmeq,
        booltob, booltosb, booltos, booltous, booltoui, booltol, booltoul,
        ltop, ptol, enumsand, enumsor, enumsxor
    };

    public enum runtime_statement_type { invoke_delegate, ctor_delegate, begin_invoke_delegate, end_invoke_delegate };

    public enum generic_parameter_kind { gpk_none, gpk_class, gpk_value };

	//The document in which this node is described.
	public interface IDocument
	{
		//Full path to the file.
		string file_name
		{
			get;
		}
	}

	//The document and position in which this node is described.
    public interface ILocation
	{
		//The line on which the beginning of this element is located.
		int begin_line_num
		{
			get;
		}

		//The column in which the beginning of this element is located.
		int begin_column_num
		{
			get;
		}

		//The line on which the end of this element is located.
		int end_line_num
		{
			get;
		}

		//The column in which the end of this element is located.
		int end_column_num
		{
			get;
		}

		//The document in which this tree element is defined.
		string file_name
		{
			get;
		}
	}

	public interface ILocated
	{
		ILocation Location
		{
			 get;
		}
	}

	//Base interface for all tree node interfaces.
	public interface ISemanticNode
	{
		void visit(ISemanticVisitor visitor);
	}

	//Base class for representing definitions in a program (type definitions, variable definitions, etc.). Never instantiated directly.
	public interface IDefinitionNode : ISemanticNode
	{
		string Documentation
		{
			get;
		}
		IAttributeNode[] Attributes
		{
			get;
		}
	}

	//Base interface for classes that describe types. Never instantiated directly.
	public interface ITypeNode : IDefinitionNode
	{
		//Kind of node - basic, common, or compiled (exported).
		node_kind node_kind
		{
			get;
		}

		//Name of the type. For case-insensitive languages, stores the name as it was declared.
		string name
		{
			get;
		}

		//Base type for this type. For object, equals null.
		ITypeNode base_type
		{
			get;
		}

        bool is_value_type
        {
            get;
        }
        
        bool is_nullable_type
        {
        	get;
        }
        
        type_special_kind type_special_kind
        {
            get;
        }

        ITypeNode element_type
        {
            get;
        }

        //ssyy
        bool is_class
        {
            get;
        }

        bool IsInterface
        {
            get;
            //set;
        }
		
        bool IsAbstract
        {
        	get;
        }
        
        bool IsEnum
        {
        	get;
        }
        
        bool IsDelegate
        {
        	get;
        }
        
        List<ITypeNode> ImplementingInterfaces
        {
            get;
        }
		List<ITypeNode> ImplementingInterfacesOrEmpty
        {
            get;
        }

        //Whether this is a generic parameter
        bool is_generic_parameter
        {
            get;
        }

        bool is_generic_type_definition
        {
            get;
        }

        bool is_generic_type_instance
        {
            get;
        }

        //Whether this depends on some indefinite type
        bool depended_from_indefinite
        {
            get;
        }

        //The generic type definition that contains this parameter
        ICommonTypeNode generic_type_container
        {
            get;
        }

        ICommonFunctionNode common_generic_function_container
        {
            get;
        }
        //\ssyy
	}

	//Interface for describing basic types.
	//This class is not needed for .NET code generation. It may be useful, for example, for native code generation.
	//There it could be used, for example, to represent integers.
	//Although in that case it may be more convenient to use compiled_type_node.
	//The name and base_type properties may be undefined for it.
	//In general, we'll think about it when needed. For now this interface should not be used anywhere.
	public interface IBasicTypeNode : ITypeNode
	{

	}

    public interface IUnsizedArray : ITypeNode
    {
        ITypeNode element_type
        {
            get;
        }
    }

    //Type synonyms defined by the user in the program.
    public interface ITypeSynonym : IDefinitionNode, ILocated
    {
        //Name of the type. For case-insensitive languages, stores the name as it was declared.
        string name
        {
            get;
        }

        //The type being aliased
        ITypeNode original_type
        {
            get;
        }

    }

    public interface ITemplateClass : IDefinitionNode
    {
        byte[] serialized_tree
        {
            get;
        }

        string name
        {
            get;
        }
    }

    //Interface for generic types
    public interface IGenericInstance
    {
        List<ITypeNode> generic_parameters
        {
            get;
        }
    }

    //Interface for generic instantiations
    public interface IGenericTypeInstance: IGenericInstance, ICommonTypeNode
    {
        ITypeNode original_generic
        {
            get;
        }

        System.Collections.Hashtable used_members
        {
            get;
        }
    }

    public interface ICommonGenericTypeInstance : IGenericTypeInstance
    {
    }

    public interface ICompiledGenericTypeInstance : IGenericTypeInstance
    {
    }

    public interface IGenericFunctionInstance : IGenericInstance, ICommonFunctionNode
    {
        IFunctionNode original_function
        {
            get;
        }
    }

    public interface ICompiledGenericMethodInstance : IGenericFunctionInstance, ICommonMethodNode
    {
    }

    //Describes ordinary types defined by the user in the program.
	public interface ICommonTypeNode : ITypeNode, INamespaceMemberNode, ILocated
	{
        bool IsSealed
        {
            get;
        }
		//Type is public or internal.
		type_access_level type_access_level
		{
			get;
		}

		//Methods of the type.
		ICommonMethodNode[] methods
		{
			get;
		}

		//Fields of the type.
		ICommonClassFieldNode[] fields
		{
			get;
		}

		//Properties of the type.
		ICommonPropertyNode[] properties
		{
			get;
		}

        //Constants defined in the type.
        IClassConstantDefinitionNode[] constants
        {
            get;
        }
		
        ICommonEventNode[] events
        {
        	get;
        }
        
        IPropertyNode default_property
        {
            get;
        }
		
        IConstantNode lower_value
        {
        	get;
        }
        
        IConstantNode upper_value
        {
        	get;
        }
        
        ICommonMethodNode static_constructor
        {
        	get;
        }
        
        int rank
        {
        	get;
        }
        
        //(ssyy) Whether this is a generic type definition
        bool is_generic_type_definition
        {
            get;
        }

        List<ICommonTypeNode> generic_params
        {
            get;
        }

        ICommonClassFieldNode runtime_initialization_marker
        {
            get;
        }

        bool has_static_constructor
        {
            get;
        }
	}

    //Interface for describing types exported from an assembly.
	//To represent compiled types I access System.Reflection directly.
	//This ties our system to .NET. It is possible to remove this dependency by introducing an additional abstraction layer.
	//To do that we need to determine what data we require from compiled types and create another intermediary layer
	//between System.Reflection and our types. Then when generating code for another platform (e.g. native code)
	//we would only need to bind the intermediary layer to a different data source. For example, type information would
	//come not from assemblies but from DLL libraries. I cannot create this layer right now because I have not yet defined
	//what queries are needed against compiled modules. For now I am using System.Reflection which seems to have everything.
	//The question of creating this intermediary layer will need to be discussed later.
	public interface ICompiledTypeNode : ITypeNode
	{
		//The compiled type.
		System.Type compiled_type
		{
			get;
		}
		
		int rank
		{
			get;
		}
	}

	//Interface representing a zero-indexed array.
	public interface IRefTypeNode : ITypeNode
	{
		ITypeNode pointed_type
		{
			get;
		}
	}
	
	public interface IShortStringTypeNode : ITypeNode
	{
		int Length
		{
			get;
		}
	}
	
	public interface ISimpleArrayNode : ITypeNode
	{
		int length
		{
			get;
		}
		
		ITypeNode element_type
		{
			get;
		}
	}
	
	public interface ISimpleArrayIndexingNode : IAddressedExpressionNode
	{
		IExpressionNode array
		{
			get;
		}
		
		IExpressionNode[] indices
		{
			get;
		}
		
		IExpressionNode index
		{
			get;
		}
	}

	//Base interface for statements. Objects implementing only this interface should never be instantiated.
	//Only classes implementing interfaces derived from this interface should be instantiated.
	//Sounds very confusing, but in general what is written above is not very important :-).
	public interface IStatementNode : ISemanticNode, ILocated
	{

	}

    public interface IRuntimeManagedMethodBody : IStatementNode
    {
        runtime_statement_type runtime_statement_type
        {
            get;
        }
    }

	//Base interface for expressions.
	public interface IExpressionNode : IStatementNode
	{
		//Type of the expression.
		ITypeNode type
		{
			get;
		}
		
		ITypeNode conversion_type
		{
			get;
		}
	}

	//Base interface for function calls. Never instantiated.
	public interface IFunctionCallNode : IExpressionNode
	{
		//List of actual parameters. The count and types of formal and actual parameters are verified
		//during semantic tree construction. Type conversion nodes are inserted into the semantic tree
		//as needed during construction.
		IExpressionNode[] real_parameters
		{
			get;
		}

		//The method being called.
		IFunctionNode function
		{
			get;
		}

        //ssyy
        //Needed for generating inherited interface functions
        bool last_result_function_call
        {
            get;
            set;
        }
        //\ssyy

	}

	//Call to a basic method.
	public interface IBasicFunctionCallNode : IFunctionCallNode
	{
		//The method being called.
		IBasicFunctionNode basic_function
		{
			get;
		}
	}

    public interface INonStaticMethodCallNode : IFunctionCallNode
    {
        bool virtual_call
        {
            get;
            set;
        }

    }

	//Call to a function defined in a namespace.
	public interface ICommonNamespaceFunctionCallNode : IFunctionCallNode
	{
		//The method being called.
		ICommonNamespaceFunctionNode namespace_function
		{
			get;
		}
	}

	//Call to a function defined inside another function.
	public interface ICommonNestedInFunctionFunctionCallNode : IFunctionCallNode
	{
		//The method being called.
		ICommonNestedInFunctionFunctionNode common_function
		{
			get;
		}

		//Static nesting depth of the nested function.
		int static_depth
		{
			get;
		}
	}

	//Call to a class method.
    public interface ICommonMethodCallNode : INonStaticMethodCallNode
	{
		//The method being called.
		ICommonMethodNode method
		{
			get;
		}

		//The class instance whose method is to be called.
		IExpressionNode obj
		{
			get;
		}
	}

	//Node corresponding to the 'this' pointer in the program.
	public interface IThisNode : IExpressionNode
	{
	}

    public interface IAsNode : IExpressionNode
    {
        IExpressionNode left
        {
            get;
        }

        ITypeNode right
        {
            get;
        }
    }

    public interface IIsNode : IExpressionNode
    {
        IExpressionNode left
        {
            get;
        }

        ITypeNode right
        {
            get;
        }
    }
    
    public interface ISizeOfOperator : IExpressionNode
    {
        ITypeNode oftype
        {
            get;
        }
    }

    public interface ITypeOfOperator : IExpressionNode
    {
        ITypeNode oftype
        {
            get;
        }
    }

	//Call to a static class method.
    public interface ICommonStaticMethodCallNode : IFunctionCallNode
	{
		//The method being called.
		ICommonMethodNode static_method
		{
			get;
		}

		//The type whose static method is being called.
		ICommonTypeNode common_type
		{
			get;
		}
	}

	//Call to a common-class constructor.
	public interface ICommonConstructorCall : ICommonStaticMethodCallNode
	{
        //ssyy
        bool new_obj_awaited();
        //\ssyy
	}

	//Call to a compiled method.
    public interface ICompiledMethodCallNode : INonStaticMethodCallNode
	{
		//The method being called.
		ICompiledMethodNode compiled_method
		{
			get;
		}

		//The class instance whose method is to be called.
		IExpressionNode obj
		{
			get;
		}

    }

	//Call to a static method of a compiled class.
    public interface ICompiledStaticMethodCallNode : IFunctionCallNode
	{
		//The method being called.
		ICompiledMethodNode static_method
		{
			get;
		}

		//The type whose static method we are calling.
		ICompiledTypeNode compiled_type
		{
			get;
		}

        ITypeNode[] template_parametres
        {
            get;
        }
	}

	//Call to a compiled type's constructor.
	public interface ICompiledConstructorCall : IFunctionCallNode
	{
		ICompiledConstructorNode constructor
		{
			get;
		}

		ICompiledTypeNode compiled_type
		{
			get;
		}

        //ssyy
        bool new_obj_awaited();
        //\ssyy
	}

	//Base interface for describing functions. Never instantiated.
	public interface IFunctionNode : IDefinitionNode
	{
		//Kind of node - basic, common, or compiled (exported).
		node_kind node_kind
		{
			get;
		}

		//List of formal parameters of the function.
		IParameterNode[] parameters
		{
			get;
		}

		//Return value type of the function.
		ITypeNode return_value_type
		{
			get;
		}

		//Name of the function. For case-insensitive languages - as the function was defined.
		string name
		{
			get;
		}

		//Location of the function - inside a function, a class, or a namespace.
		node_location_kind node_location_kind
		{
			get;
		}

        //Whether this is a generic function
        bool is_generic_function
        {
            get;
        }

        //Number of type parameters for a generic function. 0 for non-generic.
        int generic_parameters_count
        {
            get;
        }
    }

	//Interface for class members.
	public interface IClassMemberNode
	{
		//The type containing this class member.
		ITypeNode comperehensive_type
		{
			get;
		}

		//Static, common, or virtual method.
		polymorphic_state polymorphic_state
		{
			get;
		}

		//Access level for the class member.
		field_access_level field_access_level
		{
			get;
		}
	}

	//Interface for members of a compiled class.
	public interface ICompiledClassMemberNode : IClassMemberNode
	{
		//The type containing this class member.
		ICompiledTypeNode comprehensive_type
		{
			get;
		}
	}

	//Interface for members of a common class.
	public interface ICommonClassMemberNode : IClassMemberNode
	{
		//The type containing this class member.
		ICommonTypeNode common_comprehensive_type
		{
			get;
		}
	}

	//Interface for a variable or function defined inside a function.
	public interface IFunctionMemberNode
	{
		//The function containing this object.
		ICommonFunctionNode function
		{
			get;
		}
	}

	//Interface for a variable or function defined in a namespace.
	public interface INamespaceMemberNode
	{
		//The namespace in which this element is defined.
		ICommonNamespaceNode comprehensive_namespace
		{
			get;
		}
	}

	//Class for describing basic functions that are not defined anywhere (e.g., addition of two integers).
	public interface IBasicFunctionNode : IFunctionNode
	{
		//Which specific basic function this is.
		basic_function_type basic_function_type
		{
			get;
		}
	}

    public enum SpecialFunctionKind
    {
        None, New, Dispose, NewArray
    }

	//Interface for describing a user-defined function.
	public interface ICommonFunctionNode : IFunctionNode, ILocated
	{
        SpecialFunctionKind SpecialFunctionKind
        {
            get;
        }
		
        bool is_overload
        {
        	get;
        }
        
		//List of variables defined in the function.
		ILocalVariableNode[] var_definition_nodes
		{
			get;
		}

		//List of nested functions.
		ICommonNestedInFunctionFunctionNode[] functions_nodes
		{
			get;
		}

		//The function body.
		IStatementNode function_code
		{
			get;
		}

		//The variable that holds the function's return value. For procedures - null.
		ILocalVariableNode return_variable
		{
			get;
		}

        //Constants defined in the function.
        ICommonFunctionConstantDefinitionNode[] constants
        {
            get;
        }

        //Generic parameters of the function
        List<ICommonTypeNode> generic_params
        {
            get;
        }
    }

	//A function defined directly in a namespace.
	public interface ICommonNamespaceFunctionNode : ICommonFunctionNode, INamespaceMemberNode
	{
		//The namespace in which this function is defined.
		ICommonNamespaceNode namespace_node
		{
			get;
		}

        ITypeNode ConnectedToType
        { 
            get;  
        }
	}

	//A function defined inside another function.
	public interface ICommonNestedInFunctionFunctionNode : ICommonFunctionNode, IFunctionMemberNode
	{
	
	}

	//A user-defined class method.
	public interface ICommonMethodNode : ICommonFunctionNode, ICommonClassMemberNode
	{
		bool is_constructor
		{
			get;
		}

        IFunctionNode overrided_method
        {
            get;
        }

        bool is_final
        {
            get;
            set;
        }

        bool newslot_awaited
        {
            get;
            set;
        }

    }

	//Class for describing exported (compiled) functions.
	public interface ICompiledMethodNode : IFunctionNode, ICompiledClassMemberNode
	{
		//The compiled method.
		System.Reflection.MethodInfo method_info
		{
			get;
		}

        bool is_extension
        {
            get;
        }
	}

	//Call to a compiled type's constructor.
	public interface ICompiledConstructorNode : IFunctionNode, ICompiledClassMemberNode
	{
		System.Reflection.ConstructorInfo constructor_info
		{
			get;
		}
	}

	//Interface for describing an if statement.
	public interface IIfNode : IStatementNode
	{
		//The condition.
		IExpressionNode condition
		{
			get;
		}

		//The then body.
		IStatementNode then_body
		{
			get;
		}

		//The else body. If the if has no else, this property is null.
		IStatementNode else_body
		{
			get;
		}
	}

	//Interface for a while statement.
	public interface IWhileNode : IStatementNode
	{
		//The condition.
		IExpressionNode condition
		{
			get;
		}

		//The while body.
		IStatementNode body
		{
			get;
		}
	}

	//Interface for a repeat statement.
	public interface IRepeatNode : IStatementNode
	{
		//The body of do..while (repeat..until).
		IStatementNode body
		{
			get;
		}

		//The condition.
		IExpressionNode condition
		{
			get;
		}
	}

	//Class for describing a for statement.
	//This is a C++/C#-style for. Pascal's for loop is converted into this construct.
	public interface IForNode : IStatementNode
	{
		//Loop variable initialization.
		IStatementNode initialization_statement
		{
			get;
		}

		//Loop continuation condition.
		IExpressionNode while_expr
		{
			get;
		}
		
		IExpressionNode init_while_expr
		{
			get;
		}
		
		//Loop counter update.
		IStatementNode increment_statement
		{
			get;
		}

		//Loop body.
		IStatementNode body
		{
			get;
		}
		
		bool IsBoolCycle
		{
			get;
		}
	}



	public interface IWhileBreakNode : IStatementNode
	{
		IWhileNode while_node
		{
			get;
		}
	}

	public interface IRepeatBreakNode : IStatementNode
	{
		IRepeatNode repeat_node
		{
			get;
		}
	}

	public interface IForBreakNode : IStatementNode
	{
		IForNode for_node
		{
			get;
		}
	}
	
	public interface IForeachBreakNode : IStatementNode
	{
		IForeachNode foreach_node
		{
			get;
		}
	}
	
	
    public interface IExitProcedure : IStatementNode
    {
    }


	public interface IWhileContinueNode : IStatementNode
	{
		IWhileNode while_node
		{
			get;
		}
	}

	public interface IRepeatContinueNode : IStatementNode
	{
		IRepeatNode repeat_node
		{
			get;
		}
	}

	public interface IForContinueNode : IStatementNode
	{
		IForNode for_node
		{
			get;
		}
	}
	
	public interface IForeachContinueNode : IStatementNode
	{
		IForeachNode foreach_node
		{
			get;
		}
	}
	
	public interface IExternalStatementNode : IStatementNode
	{
		string module_name
		{
			get;
		}
		
		string name
		{
			get;
		}
	}
	
	public interface IPInvokeStatementNode : IStatementNode
	{
		
	}
	
    public interface ISwitchNode : IStatementNode
    {
        IExpressionNode case_expression
        {
            get;
        }

        ICaseVariantNode[] case_variants
        {
            get;
        }

        IStatementNode default_statement
        {
            get;
        }
    }

    public interface ICaseVariantNode : IStatementNode
    {
        IIntConstantNode[] elements
        {
            get;
        }

        ICaseRangeNode[] ranges
        {
            get;
        }

        IStatementNode statement_to_execute
        {
            get;
        }
    }

    public interface ICaseRangeNode : IStatementNode
    {
        IIntConstantNode lower_bound
        {
            get;
        }

        IIntConstantNode high_bound
        {
            get;
        }
    }
	
	/*//    switch.
	public interface ISwitchNode : IStatementNode
	{
	}

	//One of the variants of the switch construct.
	public interface ICaseVariantNode : IStatementNode
	{
		//Expressions for which the code corresponding to this node should be executed.
		IExpressionNode[] expressions
		{
			get;
		}

		//Ranges; if the value falls within them, the code corresponding to this node should be executed.
		IRangExpression[] ranges
		{
			get;
		}

		//The code of this node.
		IStatementNode statement
		{
			get;
		}
	}

	public interface IRangeExpression : IExpressionNode
	{
		IExpressionNode lower_bound
		{
			get;
		}

		IExpressionNode upper_bound
		{
			get;
		}
	}*/

	//Interface for describing a list of statements.
	public interface IStatementsListNode : IStatementNode
	{
        ILocalBlockVariableNode[] LocalVariables
        {
            get;
        }
		//The list of statements.
		IStatementNode[] statements
		{
			get;
		}
        //Position of the left logical bracket
        ILocation LeftLogicalBracketLocation
        {
			get;
		}
        //Position of the right logical bracket
        ILocation RightLogicalBracketLocation
        {
			get;
		}
        
	}

    public interface IThrowNode : IStatementNode
    {
        IExpressionNode exception_expresion
        {
            get;
        }
    }

    public interface ITryBlockNode : IStatementNode
    {
        IStatementNode TryStatements
        {
            get;
        }

        IStatementNode FinallyStatements
        {
            get;
        }

        IExceptionFilterBlockNode[] ExceptionFilters
        {
            get;
        }
    }

    public interface IExceptionFilterBlockNode : IStatementNode
    {
        ITypeNode ExceptionType
        {
            get;
        }

        ILocalBlockVariableReferenceNode ExceptionInstance
        {
            get;
        }

        IStatementNode ExceptionHandler
        {
            get;
        }
    }

    /*
    public interface ICatchNode : IStatementNode
    {
        ISemanticNode[] catch_body
        {
            get;
        }
    }

    public interface ITryStatementNode : IStatementNode
    {
        IStatementNode[] try_body
        {
            get;
        }

        
    }
    */
     
	//Namespace node.
	public interface INamespaceNode : IDefinitionNode
	{
		string namespace_name
		{
			get;
		}
	}
	
	//Node for a user-defined namespace.
	public interface ICommonNamespaceNode : INamespaceNode, ILocated
	{
		//Namespaces nested inside this namespace.
		ICommonNamespaceNode[] nested_namespaces
		{
			get;
		}

		//The namespace in which this namespace is nested.
		INamespaceNode comprehensive_namespace
		{
			get;
		}

		//Types described in this namespace.
		ICommonTypeNode[] types
		{
			get;
		}
		
		ITypeSynonym[] type_synonims
		{
			get;
		}

        ITemplateClass[] templates
        {
            get;
        }

		//Variables described in this namespace.
		ICommonNamespaceVariableNode[] variables
		{
			get;
		}

		//Functions described in this namespace.
		ICommonNamespaceFunctionNode[] functions
		{
			get;
		}

		//Constants described in this namespace. They must be exported somehow.
		INamespaceConstantDefinitionNode[] constants
		{
			get;
		}

        ICommonNamespaceEventNode[] events
        {
            get;
        }
		
		bool IsMain
		{
			get;
		}
	}

	//A compiled namespace.
	public interface ICompiledNamespaceNode : INamespaceNode
	{
		
	}

    /// Base interface for programs and DLLs.
    public interface IProgramBase : IDefinitionNode, ILocated
    {
        //Namespaces contained in the program or DLL.
        ICommonNamespaceNode[] namespaces
        {
            get;
        }
        
        string[] UsedNamespaces
        {
        	get;
        }
    }

	//DLL library node.
	public interface IDllNode : IProgramBase
	{
		//DLL initialization method.
		ICommonNamespaceFunctionNode initialization_function
		{
			get;
		}

		//DLL finalization method.
		ICommonNamespaceFunctionNode finalization_function
		{
			get;
		}
	}

	//Root node of the program.
	public interface IProgramNode : IProgramBase
	{
		//The main function. Executing it is equivalent to executing the program.
		//It includes calls to module initialization methods (at the start), execution of the main program body,
		//and calls to module finalization methods.
		ICommonNamespaceFunctionNode main_function
		{
			get;
		}

        //Instantiations of generic types used in the program.
        List<IGenericTypeInstance> generic_type_instances
        {
            get;
        }

        //Instantiations of generic function instances used in the program.
        List<IGenericFunctionInstance> generic_function_instances
        {
            get;
        }
        
        IStatementNode InitializationCode
        {
        	get;
        }
    }

	//Type of expressions that can return an address (e.g., a variable).
	public interface IAddressedExpressionNode : IExpressionNode
	{
		
	}

	//Return statement.
	public interface IReturnNode : IStatementNode
	{
		IExpressionNode return_value
		{
			get;
		}
	}

    //ssyy added
    //Return statement from .ctor.
    /*public interface ICtorReturnNode : IStatementNode
    {
    }*/
    //\ssyy

    public interface IReferenceNode : IAddressedExpressionNode
    {
        //The local variable definition.
        IVAriableDefinitionNode Variable
        {
            get;
        }
    }

    //Interface representing a reference to a local variable in the program body.
    public interface ILocalVariableReferenceNode : IReferenceNode
	{
		//The local variable definition.
		ILocalVariableNode variable
		{
			get;
		}

		//The difference in static nesting depth between the definition and the reference.
		int static_depth
		{
			get;
		}
	}

    //Interface representing a reference to a local variable in a block.
    public interface ILocalBlockVariableReferenceNode : IReferenceNode
    {
        //The local variable definition.
        ILocalBlockVariableNode Variable
        {
            get;
        }
    }
    
    //Interface representing a reference to a variable defined directly in a namespace.
    public interface INamespaceVariableReferenceNode : IReferenceNode
	{
		//The variable.
		ICommonNamespaceVariableNode variable
		{
			get;
		}
	}

	//Interface representing a reference to a class field.
    public interface ICommonClassFieldReferenceNode : IReferenceNode
	{
		//The class field.
		ICommonClassFieldNode field
		{
			get;
		}

		//The class object.
		IExpressionNode obj
		{
			get;
		}
	}

	//Interface representing a reference to a static class field.
    public interface IStaticCommonClassFieldReferenceNode : IReferenceNode
	{
		//The static class field.
		ICommonClassFieldNode static_field
		{
			get;
		}

		//The class whose static field we are accessing.
		ICommonTypeNode class_type
		{
			get;
		}
	}

	//Reference to a field of a compiled class.
    public interface ICompiledFieldReferenceNode : IReferenceNode
	{
		//The class field.
		ICompiledClassFieldNode field
		{
			get;
		}

		//The class object.
		IExpressionNode obj
		{
			get;
		}
	}

	//Interface representing a reference to a static field of a compiled class.
    public interface IStaticCompiledFieldReferenceNode : IReferenceNode
	{
		//The class field.
		ICompiledClassFieldNode static_field
		{
			get;
		}

		//The class whose static field we are accessing.
		ICompiledTypeNode class_type
		{
			get;
		}
	}

	//Reference to a method parameter.
    public interface ICommonParameterReferenceNode : IReferenceNode
	{
		//The method parameter being referenced.
		ICommonParameterNode parameter
		{
			get;
		}

		//The difference in static nesting depth between the parameter reference and the method in which it is declared.
		int static_depth
		{
			get;
		}
	}

	//Base node for representing constants in the program body (not named constants, but numbers, strings, etc.).
	public interface IConstantNode : IExpressionNode
	{
        object value
        {
            get;
        }
	}

	//Interface for representing boolean constants.
	public interface IBoolConstantNode : IConstantNode
	{
		//Value of the constant.
		bool constant_value
		{
			get;
		}
	}

    //Interface for representing byte constants.
    public interface IByteConstantNode : IConstantNode
    {
        //Value of the constant.
        byte constant_value
        {
            get;
        }
    }

    //Interface for representing signed byte constants.
    public interface ISByteConstantNode : IConstantNode
    {
        //Value of the constant.
        sbyte constant_value
        {
            get;
        }
    }

    //Interface for representing signed short constants.
    public interface IShortConstantNode : IConstantNode
    {
        //Value of the constant.
        short constant_value
        {
            get;
        }
    }

    //Interface for representing unsigned short constants.
    public interface IUShortConstantNode : IConstantNode
    {
        //Value of the constant.
        ushort constant_value
        {
            get;
        }
    }

    //Interface for representing int constants.
    public interface IIntConstantNode : IConstantNode
    {
        //Value of the constant.
        int constant_value
        {
            get;
        }
    }

    //Interface for representing BigInteger constants.
    public interface IBigIntConstantNode : IConstantNode
    {
        //Value of the constant.
        System.Numerics.BigInteger constant_value
        {
            get;
        }
    }

    //Interface for representing unsigned int constants.
    public interface IUIntConstantNode : IConstantNode
    {
        //Value of the constant.
        uint constant_value
        {
            get;
        }
    }

    //Interface for representing long constants.
    public interface ILongConstantNode : IConstantNode
    {
        //Value of the constant.
        long constant_value
        {
            get;
        }
    }

    //Interface for representing unsigned long constants.
    public interface IULongConstantNode : IConstantNode
    {
        //Value of the constant.
        ulong constant_value
        {
            get;
        }
    }

    //Interface for representing float constants.
    public interface IFloatConstantNode : IConstantNode
    {
        //Value of the constant.
        float constant_value
        {
            get;
        }
    }

	//Interface for representing double constants.
	public interface IDoubleConstantNode : IConstantNode
	{
		//Value of the constant.
		double constant_value
		{
			get;
		}
	}

	//Interface for representing char constants (this class is for 2-byte char - widechar in Delphi).
	public interface ICharConstantNode : IConstantNode
	{
		//Value of the constant.
		char constant_value
		{
			get;
		}
	}

	//Interface for representing string constants.
	public interface IStringConstantNode : IConstantNode
	{
		//Value of the constant.
		string constant_value
		{
			get;
		}
	}

    public interface IEnumConstNode : IConstantNode
    {
        int constant_value
        {
            get;
        }
    }

    public interface IArrayConstantNode : IConstantNode
    {
        IConstantNode[] ElementValues
        {
            get;
        }
        
        ITypeNode ElementType
        {
            get;
        }
    }
    
    public interface IArrayInitializer : IExpressionNode
    {
    	IExpressionNode[] ElementValues
        {
            get;
        }
        
        ITypeNode ElementType
        {
            get;
        }
    }
    
    public interface IRecordConstantNode : IConstantNode
    {
        IConstantNode[] FieldValues
        {
            get;
        }
    }
	
    public interface IRecordInitializer : IExpressionNode
    {
    	IExpressionNode[] FieldValues
    	{
    		get;
    	}
    }
    
	public interface ICommonStaticMethodCallNodeAsConstant : IConstantNode
    {
		ICommonStaticMethodCallNode MethodCall
		{
			get;
		}
	}

	public interface ICompiledStaticMethodCallNodeAsConstant : IConstantNode
    {
        ICompiledStaticMethodCallNode MethodCall
        {
            get;
        }
    }

    public interface ICompiledStaticFieldReferenceNodeAsConstant : IConstantNode
    {
        IStaticCompiledFieldReferenceNode FieldReference
        {
            get;
        }
    }

    public interface ICommonNamespaceFunctionCallNodeAsConstant : IConstantNode
    {
        ICommonNamespaceFunctionCallNode MethodCall
        {
            get;
        }
    }
    
    public interface IBasicFunctionCallNodeAsConstant : IConstantNode
    {
    	IBasicFunctionCallNode MethodCall
    	{
    		get;
    	}
    }

    public interface IDefaultOperatorNodeAsConstant : IConstantNode
    {
        IDefaultOperatorNode DefaultOperator
        {
            get;
        }
    }

	public interface ITypeOfOperatorAsConstant : IConstantNode
	{
		ITypeOfOperator TypeOfOperator
		{
			get;
		}
	}

	public interface ISizeOfOperatorAsConstant : IConstantNode
    {
		ISizeOfOperator SizeOfOperator
        {
			get;
        }
    }

	public interface ICompiledConstructorCallAsConstant : IConstantNode
    {
        ICompiledConstructorCall MethodCall
        {
            get;
        }
    }
    /*public interface IClassConstantNode : IConstantNode
    {

    }*/

	/*//Node for representing an assignment statement.
	public interface IAssignNode : IExpressionNode
	{
		//The assignment target.
		IAddressedExpressionNode to
		{
			get;
		}

		//The value being assigned.
		IExpressionNode from
		{
			get;
		}
	}*/

	//Base interface for formal function parameters, local variables, global program/module variables, and class fields. Never instantiated.
	public interface IVAriableDefinitionNode : IDefinitionNode
	{
		//Variable name.
		string name
		{
			get;
		}

		//Variable type.
		ITypeNode type
		{
			get;
		}

        IExpressionNode inital_value
        {
            get;
        }

		//Location of the variable.
		node_location_kind node_location_kind
		{
			get;
		}
	}

	//Interface for describing local variables.
	public interface ILocalVariableNode : IVAriableDefinitionNode, IFunctionMemberNode, ILocated
	{
		//Whether the variable is used in nested functions. Used for optimization.
		bool is_used_as_unlocal
		{
			get;
		}
	}
    
    //Interface for describing local block variables.
    public interface ILocalBlockVariableNode : IVAriableDefinitionNode, ILocated
    {
        IStatementsListNode Block
        {
            get;
        }
    }

	//Interface representing a global variable described in a module or program.
	public interface ICommonNamespaceVariableNode : IVAriableDefinitionNode, INamespaceMemberNode, ILocated
	{
		
	}

	//Interface for describing class fields.
	public interface ICommonClassFieldNode: IVAriableDefinitionNode, ICommonClassMemberNode, ILocated
	{

	}

	//A variable defined in a compiled class.
	public interface ICompiledClassFieldNode : IVAriableDefinitionNode, ICompiledClassMemberNode
	{
		System.Reflection.FieldInfo compiled_field
		{
			get;
		}
	}

	//Base interface for interfaces representing parameters of basic, common, and compiled functions.
	public interface IParameterNode : IVAriableDefinitionNode
	{
		//Type of the parameter.
		parameter_type parameter_type
		{
			get;
		}

		//The function in which this parameter is described.
		IFunctionNode function
		{
			get;
		}

        bool is_params
        {
            get;
        }

        bool is_const
        {
            get;
        }

        IExpressionNode default_value
        {
            get;
        }
	}

	//Interface for representing parameters of common functions.
	public interface ICommonParameterNode : IParameterNode, ILocated
	{
		//The function in which this parameter is defined.
		ICommonFunctionNode common_function
		{
			get;
		}

		//Whether the parameter is used in nested functions.
		bool is_used_as_unlocal
		{
			get;
		}
	}

	//Interface representing parameters of basic functions.
	public interface IBasicParameterNode : IParameterNode
	{
		
	}

	//Interface representing parameters of compiled functions.
	public interface ICompiledParameterNode : IParameterNode
	{
		//The function in which this parameter is defined.
		ICompiledMethodNode compiled_function
		{
			get;
		}
	}

	//Interface describing a constant definition.
	public interface IConstantDefinitionNode : IDefinitionNode
	{
		//Constant name.
		string name
		{
			get;
		}

		//Constant type.
		ITypeNode type
		{
			get;
		}

		//Constant value.
		IConstantNode constant_value
		{
			get;
		}
	}

    //A constant defined in a class.
    public interface IClassConstantDefinitionNode : IConstantDefinitionNode, IClassMemberNode, ILocated
    {

    }

    public interface ICompiledClassConstantDefinitionNode : IConstantDefinitionNode, IClassMemberNode
    {
        ICompiledTypeNode comprehensive_type
        {
            get;
        }
    }

    //A constant defined in a namespace.
    public interface INamespaceConstantDefinitionNode : IConstantDefinitionNode, ILocated
    {
        ICommonNamespaceNode comprehensive_namespace
        {
            get;
        }
    }

    //A constant defined in a function.
    public interface ICommonFunctionConstantDefinitionNode : IConstantDefinitionNode, ILocated
    {
        ICommonFunctionNode comprehensive_function
        {
            get;
        }
    }

    //A constant defined in a compiled type.
    public interface ICompiledConstantNode : IConstantDefinitionNode
    {
        ICompiledTypeNode comprehensive_type
        {
            get;
        }
    }

	//Node describing a class property. Never instantiated.
	public interface IPropertyNode : IDefinitionNode
	{
		//Kind of object (basic, common, compiled).
		node_kind node_kind
		{
			get;
		}

		//Name of the property.
		string name
		{
			get;
		}

		//The type that contains this property.
		ITypeNode comprehensive_type
		{
			get;
		}

		//Type of the property.
		ITypeNode property_type
		{
			get;
		}

		//The function that returns the property value.
		IFunctionNode get_function
		{
			get;
		}

		//The function that sets the property value.
		IFunctionNode set_function
		{
			get;
		}

		IParameterNode[] parameters
		{
			get;
		}
	}

	//A user-defined property.
	public interface ICommonPropertyNode : IPropertyNode, ICommonClassMemberNode, ILocated
	{
		//The type that contains this property.
		/*ICommonTypeNode common_comprehensive_type
		{
			get;
		}*/

		//The function that returns the property value.
		/*ICommonClassMemberNode get_common_function
		{
			get;
		}

		//The function that sets the property value.
		ICommonClassMemberNode set_common_function
		{
			get;
		}*/
	}

	//Basic property. Not needed anywhere yet, but may be very useful for native code generation.
	public interface IBasicPropertyNode : IPropertyNode
	{

	}

	//Property in a compiled type.
	public interface ICompiledPropertyNode : IPropertyNode, ICompiledClassMemberNode
	{
		//The property in the assembly.
		System.Reflection.PropertyInfo property_info
		{
			get;
		}

		//The type that contains this property.
		ICompiledTypeNode compiled_comprehensive_type
		{
			get;
		}

		//The function that returns the property value.
		ICompiledMethodNode compiled_get_method
		{
			get;
		}

		//The function that sets the property value.
		ICompiledMethodNode compiled_set_method
		{
			get;
		}

	}

	public interface IGetAddrNode : IExpressionNode
	{
		IExpressionNode addr_of_expr
		{
			get;
		}
	}
	
	public interface IDereferenceNode : IAddressedExpressionNode
	{
		IExpressionNode derefered_expr
		{
			get;
		}
	}

	public interface INullConstantNode : IConstantNode
    {
    }

    public interface IStatementsExpressionNode : IExpressionNode
    {
        IStatementNode[] statements
        {
            get;
        }

        IExpressionNode expresion
        {
            get;
        }
    }

    public interface IQuestionColonExpressionNode : IExpressionNode
    {
        IExpressionNode condition
        {
            get;
        }

        IExpressionNode ret_if_true
        {
            get;
        }

        IExpressionNode ret_if_false
        {
            get;
        }
    }

	public interface IDoubleQuestionColonExpressionNode : IExpressionNode
	{
		IExpressionNode condition
		{
			get;
		}

		IExpressionNode ret_if_null
		{
			get;
		}
	}

	public interface ILabelNode : IDefinitionNode, ILocated
    {
        //Label name. For case-insensitive languages, stores the name as it was declared.
        string name
        {
            get;
        }

        //whether the label was encountered in the code
        /*bool is_defined
        {
            get;
            set;
        }*/

    }

    public interface ILabeledStatementNode : IStatementNode, ILocated
    {
        //The label that marks this statement
        ILabelNode label
        {
            get;
        }

        //The statement itself
        IStatementNode statement
        {
            get;
        }

    }

    public interface IGotoStatementNode : IStatementNode, ILocated
    {
        //The label to which the goto jumps
        ILabelNode label
        {
            get;
        }
    }

    public interface IForeachNode : IStatementNode, ILocated
    {
        IVAriableDefinitionNode VarIdent
        {
            get;
        }

        IExpressionNode InWhatExpr
        {
            get;
        }

        IStatementNode Body
        {
            get;
        }

        ITypeNode ElementType
        {
            get;
        }

        bool IsGeneric
        {
            get;
        }
    }

    public interface ILockStatement : IStatementNode, ILocated
    {
        IExpressionNode LockObject
        {
            get;
        }
        IStatementNode Body
        {
            get;
        }
    }

    public interface IRethrowStatement : IStatementNode, ILocated
    {
    	
    }
    
    public interface INamespaceConstantReference : IConstantNode, ILocated
    {
    	INamespaceConstantDefinitionNode Constant
    	{
    		get;
    	}
    }
    
    public interface IFunctionConstantReference : IConstantNode, ILocated
    {
    	ICommonFunctionConstantDefinitionNode Constant
    	{
    		get;
    	}
    }
    
    public interface IFunctionConstantDefinitionNode : IConstantDefinitionNode, ILocated
    {
        ICommonFunctionNode function
        {
            get;
        }
    }
    
    public interface ICommonConstructorCallAsConstant : IConstantNode, ILocated
    {
    	ICommonConstructorCall ConstructorCall
    	{
    		get;
    	}
    }
    
    public interface IEventNode : IDefinitionNode
    {
    	
    }
    
    public interface ICompiledEventNode : IEventNode
    {
        System.Reflection.EventInfo CompiledEvent
        {
            get;
        }
    }
    
    public interface ICommonEventNode : IDefinitionNode, IEventNode, ICommonClassMemberNode, ILocated
    {
    	string Name
    	{
    		get;
    	}
    	
    	ITypeNode DelegateType
    	{
    		get;
    	}
    	
    	ICommonMethodNode AddMethod
    	{
    		get;
    	}
    	
    	ICommonMethodNode RemoveMethod
    	{
    		get;
    	}
    	
    	ICommonMethodNode RaiseMethod
    	{
    		get;
    	}

        ICommonClassFieldNode Field
        {
            get;
        }

    	bool IsStatic
    	{
    		get;
    	}
    }

    public interface ICommonNamespaceEventNode : IDefinitionNode, IEventNode, ILocated
    {
        string Name
        {
            get;
        }

        ITypeNode DelegateType
        {
            get;
        }

        ICommonNamespaceFunctionNode AddFunction
        {
            get;
        }

        ICommonNamespaceFunctionNode RemoveFunction
        {
            get;
        }

        ICommonNamespaceFunctionNode RaiseFunction
        {
            get;
        }

        ICommonNamespaceVariableNode Field
        {
            get;
        }
    }

    public interface IStaticEventReference : IAddressedExpressionNode
    {
    	IEventNode Event
    	{
    		get;
    	}
    }
    
    public interface INonStaticEventReference : IStaticEventReference
    {
    	IExpressionNode obj
    	{
    		get;
    	}
    }

    public interface IDefaultOperatorNode : IExpressionNode
    {
    }
	
    public interface IAttributeNode : ISemanticNode, ILocated
    {
    	IFunctionNode AttributeConstructor
    	{
    		get;
    	}
    	
    	attribute_qualifier_kind qualifier
    	{
    		get;
    	}
    	
    	ITypeNode AttributeType
    	{
    		get;
    	}
    	
    	IConstantNode[] Arguments
    	{
    		get;
    	}
    	
    	IPropertyNode[] PropertyNames
    	{
    		get;
    	}
    	
    	IConstantNode[] PropertyInitializers
    	{
    		get;
    	}
    	
    	IVAriableDefinitionNode[] FieldNames
    	{
    		get;
    	}
    	
    	IConstantNode[] FieldInitializers
    	{
    		get;
    	}
    }
    public interface ILambdaFunctionNode : IExpressionNode
    {
        //Kind of node - basic, common, or compiled (exported).
        node_kind node_kind
        {
            get;
        }

        //List of formal parameters of the function.
        IParameterNode[] parameters
        {
            get;
        }

        //Return value type of the function.
        ITypeNode return_value_type
        {
            get;
        }

        IStatementNode body
        {
            get;
        }

        IFunctionNode function
        {
            get;
        }

        //Location of the function - inside a function, a class, or a namespace.
        node_location_kind node_location_kind
        {
            get;
        }

        //Whether this is a generic function
        bool is_generic_function
        {
            get;
        }

        //Number of type parameters for a generic function. 0 for non-generic.
        int generic_parameters_count
        {
            get;
        }
    }
    public interface ILambdaFunctionCallNode : IExpressionNode
    {
        //List of actual parameters. The count and types of formal and actual parameters are verified
        //during semantic tree construction. Type conversion nodes are inserted into the semantic tree
        //as needed during construction.
        IExpressionNode[] parameters
        {
            get;
        }

        //The method being called.
        ILambdaFunctionNode lambda
        {
            get;
        }
    }
    /*public interface ICompiledFunctionNode : IFunctionNode
    {
        string test
        {
            get;
        }      
    }*/
}
