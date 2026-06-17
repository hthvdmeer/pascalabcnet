// Copyright (c) Ivan Bondarev, Stanislav Mikhalkovich (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using PascalABCCompiler.SemanticTree;
using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

namespace PascalABCCompiler.NETGenerator {
	
	public class HandlerFactory
	{
		public static ConstructorInfo ci;
		public static Type[] parameters;
		public static Type eventHandler;
		
		static HandlerFactory()
		{
			eventHandler = typeof(System.EventHandler);
			parameters = new Type[2]{typeof(object),typeof(IntPtr)};
			ci = eventHandler.GetConstructor(parameters);
		}
	}
	
	public abstract class NodeInfo
    {
		
	}

    public static class OperatorsNameConvertor
    {
        private static System.Collections.Generic.Dictionary<string, string> names =
            new System.Collections.Generic.Dictionary<string, string>(32);
        
        static OperatorsNameConvertor()
        {
            names[StringConstants.plus_name]="op_Addition";
            names[StringConstants.minus_name]="op_Subtraction";
            names[StringConstants.mul_name]="op_Multiply";
            names[StringConstants.div_name]="op_Division";
            names[StringConstants.and_name]="op_BitwiseAnd";
            names[StringConstants.or_name]="op_BitwiseOr";
            names[StringConstants.eq_name]="op_Equality";
            names[StringConstants.gr_name]="op_GreaterThan";
            names[StringConstants.greq_name]="op_GreaterThanOrEqual";
            names[StringConstants.sm_name]="op_LessThan";
            names[StringConstants.smeq_name]="op_LessThanOrEqual";
            names[StringConstants.mod_name]="op_Modulus";
            names[StringConstants.not_name]="op_LogicalNot";
            names[StringConstants.noteq_name]="op_Inequality";
            
            //op_Implicit
            //op_Explicit

            names[StringConstants.xor_name]="op_ExclusiveOr";
            names[StringConstants.and_name]="op_LogicalAnd";
            names[StringConstants.or_name]="op_LogicalOr";
            names[StringConstants.assign_name]="op_Assign";
            names[StringConstants.shl_name]="op_LeftShift";
            names[StringConstants.shr_name]="op_RightShift";
            //names["op_SignedRightShift"]=StringConstants.shr_name;
            names[StringConstants.shr_name]="op_UnsignedRightShift";
            names[StringConstants.eq_name]="op_Equality";
            names[StringConstants.multassign_name]="op_MultiplicationAssignment";
            names[StringConstants.minusassign_name]="op_SubtractionAssignment";
            //names[StringConstants.minusassign_name]="op_ExclusiveOrAssignment";
            //op_LeftShiftAssignment
            //op_ModulusAssignment
            names[StringConstants.plusassign_name]="op_AdditionAssignment";
            //op_BitwiseAndAssignment
            //op_BitwiseOrAssignment
            //op_Comma
            names[StringConstants.divassign_name]="op_DivisionAssignment";
            //op_Decrement
            //op_Increment
            //names[StringConstants.minus_name] ="op_UnaryNegation";
            //op_UnaryPlus
            //op_OnesComplement

        }

        public static string convert_name(string name)
        {
            string ret;
            if (names.TryGetValue(name, out ret))
            {
                return ret;
            }
            return null;
        }
    }
	
	public class TypeInfo : NodeInfo
    {
		private Type _tp;
		private bool _is_arr=false;//flag indicating whether this is an array
		public bool is_set=false;
		public bool is_typed_file=false;
		public bool is_text_file=false;
		public int arr_len;
		public ConstructorInfo def_cnstr;//default constructor of the type (if one exists)
		public FieldInfo arr_fld;//reference to the array field in the array wrapper
		public MethodInfo clone_meth;//array copy method
		public MethodInfo init_meth;//initialization method
        public MethodInfo assign_meth;//method for assigning values of value types
        public ConstructorBuilder static_cnstr;
        public MethodBuilder fix_meth;
		//temporary, for events
		public MethodBuilder handl_meth;
		public bool has_events=false;//whether the type has events
		//public Hashtable fields=new Hashtable();//temporary
        public MethodInfo enumerator_meth;
		
		public TypeInfo() {}
		
		public TypeInfo(Type tp)
		{
			_tp = tp;
		}
		
		public Type tp {
			get {
				return _tp;
			}
			set {
				_tp = value;
			}
		}
		
		public bool is_arr {
			get 
            {
                
                
                return _is_arr;
			}
			set 
            {
				_is_arr = value;
			}
		}
	}
	
	public class EvntInfo : NodeInfo
	{
		private EventBuilder _ei;
		
		public EvntInfo(EventBuilder ei)
		{
			_ei = ei;
		}
		
		public EventBuilder ei
        {
			get {
				return _ei;
			}
			set {
				_ei = value;
			}
		}
		
	}
	
	public class FldInfo : NodeInfo {
		private FieldInfo _fi;
		
		public FldInfo() {}

        public FldInfo(FieldInfo fi)
		{
			_fi = fi;
		}

        public FieldInfo fi
        {
			get {
				return _fi;
			}
			set {
				_fi = value;
			}
		}
		
        public virtual Type field_type
        {
            get
            {
                return fi.FieldType;
            }
        }
	}

    public class GenericFldInfo : FldInfo
    {
        private Type _field_type;
        public FieldInfo prev_fi; // passed so that at the third stage in NegGenerator.cs (around line 1586) the correct type can be constructed. Workaround for #1632

        public override Type field_type
        {
            get
            {
                return _field_type;
            }
        }

        public GenericFldInfo(FieldInfo fi, Type field_type, FieldInfo prev_fi)
            : base(fi)
        {
            _field_type = field_type;
            this.prev_fi = prev_fi;
        }
	}
	
    public class PropInfo : NodeInfo {
    	private PropertyInfo _prop;
    	
    	public PropInfo(PropertyInfo _prop)
    	{
    		this._prop = _prop;
    	}
    	
    	public PropertyInfo prop
    	{
    		get
    		{
    			return _prop;
    		}
    	}
    }
    
	public class ConstrInfo : NodeInfo {
		private ConstructorInfo _ci;
		
		public ConstrInfo() {}
		
		public ConstrInfo(ConstructorInfo ci)
		{
			_ci = ci;
		}
		
		public ConstructorInfo ci {
			get {
				return _ci;
			}
			set {
				_ci = value;
			}
		}
	}
	
	public class MethInfo : NodeInfo {
		private MethodInfo _mi;
		//private LocalBuilder _ret_val;//variable for the return value //(ssyy) No use
		private LocalBuilder _frame;//variable holding the activation record
		private MethInfo _up_meth;//reference to the enclosing method
		private Frame _disp;//activation record
		private bool _nested=false;//whether this is nested or contains nested methods
		private int _num_scope;//scope index
		private ConstructorInfo _cnstr;
		private bool _stand=false;//for standard procedures that have no body in the semantic tree ("New", "Dispose")
        private bool _is_in_class = false;//whether this is a procedure nested inside a method
        private bool _is_ptr_ret_type = false;

		public MethInfo() {}
		
		public MethInfo(MethodInfo mi)
		{
			_mi = mi;
		}

        public bool is_ptr_ret_type
        {
            get
            {
                return _is_ptr_ret_type;
            }
            set
            {
                _is_ptr_ret_type = value;
            }
        }

        public bool is_in_class
        {
            get
            {
                return _is_in_class;
            }
            set
            {
                _is_in_class = value;
            }
        }

		public bool stand
		{
			get
			{
				return _stand;
			}
			set
			{
				_stand = value;
			}
		}
		
		public MethodInfo mi {
			get {
				return _mi;
			}
			set {
				_mi = value;
			}
		}
		
		public MethInfo up_meth {
			get {
				return _up_meth;
			}
			set {
				_up_meth = value;
			}
		}
		
		public ConstructorInfo cnstr {
			get {
				return _cnstr;
			}
			set {
				_cnstr = value;
			}
		}
		
		public int num_scope {
			get {
				return _num_scope;
			}
			set {
				_num_scope = value;
			}
		}
		
		public Frame disp {
			get {
				return _disp;
			}
			set {
				_disp = value;
			}
		}
		
		public bool nested {
			get {
				return _nested;
			}
			set {
				_nested = value;
			}
		}
		
		public LocalBuilder frame {
			get {
				return _frame;
			}
			set {
				_frame = value;
			}
		}
		
        //(ssyy) No use
		/*public LocalBuilder ret_val {
			get {
				return _ret_val;
			}
			set {
				_ret_val = value;
			}
		}*/
		
	}
	
	public enum VarKind 
	{
		vkLocal, //local
		vkNonLocal, //non-local (contained in a procedure)
		vkGlobal //global variable (main program)
	}
	
	public class VarInfo : NodeInfo {
		private LocalBuilder _lb;//builder for the variable
		private FieldBuilder _fb;//in case the variable is non-local
		private VarKind _kind;//variable kind
		private MethInfo _meth;//method in which the variable is defined
		
		public VarInfo() {}
		
		public VarInfo(LocalBuilder lb)
		{
			_lb = lb;
			_kind = VarKind.vkLocal;
		}
		
		public MethInfo meth {
			get {
				return _meth;
			}
			set {
				_meth = value;
			}
		}
		
		public FieldBuilder fb {
			get {
				return _fb;
			}
			set {
				_fb = value;
			}
		}
		
		public VarKind kind {
			get {
				return _kind;
			}
			set {
				_kind = value;
			}
		}
		
		public LocalBuilder lb {
			get {
				return _lb;
			}
			set {
				_lb = value;
			}
		}	
	}
	
	public enum ParamKind {
		pkNone,
		pkGlobal
	}
	
	public class ParamInfo : NodeInfo {
		private ParameterBuilder _pb;//builder for the parameter
		private FieldBuilder _fb;//in case the parameter is non-local
		private ParamKind _kind = ParamKind.pkNone;
		private MethInfo _meth;//method in which the parameter is defined
		
		public ParamInfo() {}
		
		public ParamInfo(ParameterBuilder pb)
		{
			_pb = pb;
		}
		
		public MethInfo meth {
			get {
				return _meth;
			}
			set {
				_meth = value;
			}
		}
		
		public ParamKind kind {
			get {
				return _kind;
			}
			set {
				_kind = value;
			}
		}
		
		public FieldBuilder fb {
			get {
				return _fb;
			}
			set {
				_fb = value;
			}
		}
		
		public ParameterBuilder pb {
			get {
				return _pb;
			}
			set {
				_pb = value;
			}
		}	
	}
	
	public class ConstInfo : NodeInfo {
		public FieldBuilder fb;
		
		public ConstInfo(FieldBuilder fb)
		{
			this.fb = fb;
		}
	}
	
	//Structure for the procedure activation record
	public class Frame {
		public TypeBuilder tb; //class representing the activation record
		public FieldBuilder parent; //field referencing the parent activation record
		public ConstructorBuilder cb; //constructor of the activation record
		public MethodBuilder mb;
		
		public Frame() {}
	}
	
	public class Helper {
		public Hashtable defs=new Hashtable();
        private HashSet<ICommonTypeNode> processing_types = new HashSet<ICommonTypeNode>();
		private MethodInfo arr_mi=null;
        private Dictionary<ITypeNode, Type> pas_defs = new Dictionary<ITypeNode, Type>();
        private Dictionary<IExpressionNode, LocalBuilder> memoized_exprs = new Dictionary<IExpressionNode, LocalBuilder>();
        private Dictionary<TypeBuilder, MethodBuilder> dummy_methods = new Dictionary<TypeBuilder, MethodBuilder>();

		public Helper() {}
		
		public void AddDummyMethod(TypeBuilder tb, MethodBuilder mb)
        {
			dummy_methods[tb] = mb;
        }

        public MethodBuilder GetDummyMethod(TypeBuilder tb)
        {
            return dummy_methods[tb];
        }

		public void AddPascalTypeReference(ITypeNode tn, Type t)
		{
			pas_defs[tn] = t;
		}
		
		public Type GetPascalTypeReference(ITypeNode tn)
		{
			pas_defs.TryGetValue(tn, out var result);
			return result;
		}
		
		public ConstInfo AddConstant(IConstantDefinitionNode cnst, FieldBuilder fb)
		{
			ConstInfo ci = new ConstInfo(fb);
			defs[cnst] = ci;
			return ci;
		}
		
        //add a local variable
		public VarInfo AddVariable(IVAriableDefinitionNode var, LocalBuilder lb)
		{
			VarInfo vi = new VarInfo(lb);
			defs[var] = vi;
			return vi;
		}

        //ssyy
        public Label GetLabel(ILabelNode label, ILGenerator il)
        {
            if (defs.ContainsKey(label))
            {
                return (Label)(defs[label]);
            }
            Label lab = il.DefineLabel();
            defs.Add(label, lab);
            return lab;
        }
        //\ssyy
		
        //get a local variable
		public VarInfo GetVariable(IVAriableDefinitionNode var)
		{
			return (VarInfo)defs[var];
		}
		
        //add a global variable
		public VarInfo AddGlobalVariable(IVAriableDefinitionNode var, FieldBuilder fb)
		{
			VarInfo vi = new VarInfo();
			defs[var] = vi;
			vi.fb = fb;
			vi.kind = VarKind.vkGlobal;
			return vi;
		}
		
		public EvntInfo AddEvent(IEventNode ev, EventBuilder eb)
		{
			EvntInfo ei = new EvntInfo(eb);
			defs[ev] = ei;
			return ei;
		}
		
		public EvntInfo GetEvent(IEventNode ev)
		{
			return (EvntInfo)defs[ev];
		}
		
        //add a non-local variable
		public VarInfo AddNonLocalVariable(IVAriableDefinitionNode var, FieldBuilder fb)
		{
			VarInfo vi = new VarInfo();
			defs[var] = vi;
			vi.fb = fb;
			vi.kind = VarKind.vkNonLocal;
			return vi;
		}
		
        //add a function (method)
		public MethInfo AddMethod(IFunctionNode func, MethodInfo mi)
		{
			MethInfo m = new MethInfo(mi);
			defs[func] = m;
			return m;
		}
		
        //add a function nested inside another function
		public MethInfo AddMethod(IFunctionNode func, MethodInfo mi, MethInfo up)
		{
			MethInfo m = new MethInfo(mi);
			m.up_meth = up;
			defs[func] = m;
			return m;
		}
		
        //get a method
		public MethInfo GetMethod(IFunctionNode func)
		{
			return (MethInfo)defs[func];
		}
		
        //add a constructor
		public MethInfo AddConstructor(IFunctionNode func, ConstructorInfo ci)
		{
			//ConstrInfo m = new ConstrInfo(ci);
			MethInfo mi = new MethInfo();
			mi.cnstr = ci;
			defs[func] = mi;
			return mi;
		}
		
		public PropInfo AddProperty(IPropertyNode prop, PropertyInfo pi)
		{
			PropInfo pi2 = new PropInfo(pi);
			defs[prop] = pi2;
			return pi2;
		}
		
		public PropInfo GetProperty(IPropertyNode prop)
		{
			return (PropInfo)defs[prop];
		}
		
        //get a constructor
		public MethInfo GetConstructor(IFunctionNode func)
		{
			MethInfo mi = (MethInfo)defs[func];
			return mi;
		}
		
		public ConstInfo GetConstant(IConstantDefinitionNode cnst)
		{
			ConstInfo ci = (ConstInfo)defs[cnst];
			return ci;
		}

        public object GetConstantForExpression(IExpressionNode expr)
        {
            if (expr is PascalABCCompiler.TreeRealization.null_const_node) // SSM 20/04/21
                return expr;
            if (expr is IConstantNode)
                return (expr as IConstantNode).value;
            return null;
        }

        //add a parameter
		public ParamInfo AddParameter(IParameterNode p, ParameterBuilder pb)
		{
			ParamInfo pi = new ParamInfo(pb);
			defs[p] = pi;
			return pi;
		}
		
        //add a non-local parameter
		public ParamInfo AddGlobalParameter(IParameterNode p, FieldBuilder fb)
		{
			ParamInfo pi = new ParamInfo();
			pi.kind = ParamKind.pkGlobal;
			pi.fb = fb;
			defs[p] = pi;
			return pi;
		}
		
        //get a parameter
		public ParamInfo GetParameter(IParameterNode p)
		{
			return (ParamInfo)defs[p];
		}
		
        //add a field
		public FldInfo AddField(ICommonClassFieldNode f, FieldInfo fb)
		{
			FldInfo fi = new FldInfo(fb);
#if DEBUG
            /*if (f.name == "XYZW")
            {
                var y = f.GetHashCode();
            } */
#endif
            defs[f] = fi;
            return fi;
		}
		
        public FldInfo AddGenericField(ICommonClassFieldNode f, FieldInfo fb, Type field_type, FieldInfo prev_fi)
        {
            FldInfo fi = new GenericFldInfo(fb, field_type, prev_fi); // prev_fi - to construct the correct fi at the final stage
#if DEBUG
            /*if (f.name == "XYZW")
            {
                var y = f.GetHashCode();
            }*/
#endif
            defs[f] = fi;
            return fi;
        }

        //get a field
		public FldInfo GetField(ICommonClassFieldNode f)
		{
            var r = (FldInfo)defs[f];
#if DEBUG
            /*if (f.name == "XYZW")
            {
                var y = f.GetHashCode();
            } */
#endif
#if DEBUG
            /*if (r == null && f.name == "XYZW")
            {
                foreach (var k in defs.Keys)
                {
                    if ((k is ICommonClassFieldNode) && (k as ICommonClassFieldNode).name == "XYZW")
                        return (FldInfo)defs[k];
                }
            } */
#endif
            return r;
		}
		
        //add a type
		public TypeInfo AddType(ITypeNode type, TypeBuilder tb)
		{
			TypeInfo ti = new TypeInfo(tb);
			defs[type] = ti;
			return ti;
		}
		
        public TypeInfo AddEnum(ITypeNode type, EnumBuilder emb)
        {
            TypeInfo ti = new TypeInfo(emb);
            defs[type] = ti;
            return ti;
        }

        public TypeInfo AddExistingType(ITypeNode type, Type t)
        {
            TypeInfo ti = new TypeInfo(t);
            defs[type] = ti;
            return ti;
        }
		
        private IFunctionNode find_method(ICommonTypeNode tn, string name)
        {
        	foreach (ICommonMethodNode cmn in tn.methods)
        	{
        		if (string.Compare(cmn.name,name,true) == 0) return cmn;
        	}
            return null;
        }
        
        private IFunctionNode find_constructor(ICommonTypeNode tn)
        {
        	foreach (ICommonMethodNode cmn in tn.methods)
        	{
        		if (cmn.is_constructor) return cmn;
        	}
        	return null;
        }

        private ConstructorInfo find_constructor(Type tn)
        {
            foreach (ConstructorInfo cmn in tn.GetConstructors())
            {
                return cmn;
            }
            return null;
        }

        private IFunctionNode find_constructor_with_params(ICommonTypeNode tn)
        {
        	foreach (ICommonMethodNode cmn in tn.methods)
        	{
        		if (cmn.is_constructor && cmn.parameters.Length == 2) return cmn;
        	}
        	return null;
        }

        private ConstructorInfo find_constructor_with_params(Type t)
        {
            foreach (ConstructorInfo ci in t.GetConstructors())
            {
                if (ci.GetParameters().Length == 2)
                    return ci;
            }
            return null;
        }

        private IFunctionNode find_constructor_with_one_param(ICommonTypeNode tn)
        {
        	foreach (ICommonMethodNode cmn in tn.methods)
        	{
        		if (cmn.is_constructor && cmn.parameters.Length == 1) return cmn;
        	}
        	return null;
        }

        private ConstructorInfo find_constructor_with_one_param(Type t)
        {
            foreach (ConstructorInfo ci in t.GetConstructors())
            {
                if (ci.GetParameters().Length == 1)
                    return ci;
            }
            return null;
        }

        /// <summary>
        /// Before calling <c>.CreateType()</c>, allows determining whether the type was declared in code rather than in a pre-built assembly.
        /// Generic types instantiated with Pascal types are also considered Pascal types (example: <c>IEnumerable&lt;PascalType&gt;</c>)
        /// </summary>
        public bool IsPascalType(Type t)
        {
            if (t is TypeBuilder || t is GenericTypeParameterBuilder || t is EnumBuilder || t.GetType().FullName == "System.Reflection.Emit.TypeBuilderInstantiation")
                return true;

            if ( t.IsGenericType && t.GetGenericArguments().Any(IsPascalType) )
                return true;

            if (t.IsArray)
                return IsPascalType(t.GetElementType());

            return false;
        }

        public bool IsNumericType(Type t)
        {
            return t == TypeFactory.ByteType || t == TypeFactory.SByteType || t == TypeFactory.Int16Type || t == TypeFactory.UInt16Type
                || t == TypeFactory.Int32Type || t == TypeFactory.UInt32Type || t == TypeFactory.Int64Type || t == TypeFactory.UInt64Type
                || t == TypeFactory.SingleType || t == TypeFactory.DoubleType;
        }

        public ICommonTypeNode GetTypeNodeByTypeBuilder(TypeBuilder tb)
        {
            foreach (object o in defs.Keys)
            {
                if (o is ICommonTypeNode && this.GetTypeReference(o as ICommonTypeNode).tp == tb)
                    return o as ICommonTypeNode;
            }
            return null;
        }

        public void SetAsProcessing(ICommonTypeNode type)
        {
            processing_types.Add(type);
        }

        public bool IsProcessing(ICommonTypeNode type)
        {
            return processing_types.Contains(type);
        }

        public void LinkExpressionToLocalBuilder(IExpressionNode expr, LocalBuilder lb)
        {
            memoized_exprs[expr] = lb;
        }

        public LocalBuilder GetLocalBuilderForExpression(IExpressionNode expr)
        {
            memoized_exprs.TryGetValue(expr, out var result);
            return result;
        }

        //get a type
        public TypeInfo GetTypeReference(ITypeNode type)
		{
			TypeInfo ti = defs[type] as TypeInfo;
			if (ti != null) 
			{
				if (type.type_special_kind == type_special_kind.text_file) 
					ti.is_text_file = true;
				if (!ti.is_set && !ti.is_typed_file && !ti.is_text_file)
                    return ti;
				if (ti.clone_meth == null && !ti.is_typed_file && !ti.is_text_file)
                {
                    if (type is ICommonTypeNode)
                        ti.clone_meth = this.GetMethodBuilder(find_method(type as ICommonTypeNode, "CloneSet"));//ti.tp.GetMethod("Clone");
                    else
                        ti.clone_meth = ti.tp.GetMethod("CloneSet");
                }
                if (ti.def_cnstr == null)
                {
                	//if (type.type_special_kind == type_special_kind.text_file) ti.is_text_file = true;
                    if (ti.is_set)
                    {
                        if (type is ICommonTypeNode)
                            ti.def_cnstr = this.GetConstructorBuilder(find_constructor_with_params(type as ICommonTypeNode));
                        else
                            ti.def_cnstr = find_constructor_with_params(ti.tp);
                    }
                    else if (ti.is_typed_file)
                    {
                        if (type is ICommonTypeNode)
                            ti.def_cnstr = this.GetConstructorBuilder(find_constructor_with_one_param(type as ICommonTypeNode));
                        else
                            ti.def_cnstr = find_constructor_with_one_param(ti.tp);
                    }
                    else
                    {
                        if (type is ICommonTypeNode)
                            ti.def_cnstr = this.GetConstructorBuilder(find_constructor(type as ICommonTypeNode));
                        else
                            ti.def_cnstr = find_constructor(ti.tp);
                    }
                }
                if (ti.assign_meth == null && !ti.is_typed_file && !ti.is_text_file)
                {
                    if (type is ICommonTypeNode)
                        ti.assign_meth = this.GetMethodBuilder(find_method(type as ICommonTypeNode, "AssignSetFrom"));
                    else
                        ti.assign_meth = ti.tp.GetMethod("AssignSetFrom");
                }
				return ti;
			}
			if (type is ICompiledTypeNode) {
				ti = new TypeInfo(((ICompiledTypeNode)type).compiled_type);
				defs[type] = ti;
				return ti;
			}
            //(ssyy) Sped up by inserting a switch
            switch (type.type_special_kind)
            {
                case type_special_kind.typed_file:
                    ti = GetTypeReference(type.base_type);
                    if (ti == null) return null;
                    ti.is_typed_file = true;
                    if (ti.def_cnstr == null)
                    {
                        if (type.base_type is ICommonTypeNode)
                            ti.def_cnstr = this.GetConstructorBuilder(find_constructor_with_one_param(type.base_type as ICommonTypeNode));
                        else
                            ti.def_cnstr = find_constructor_with_one_param(ti.tp);
                    }
                    return ti;
                case type_special_kind.set_type:
                    ti = GetTypeReference(type.base_type);
                    if (ti == null) return null;
                    ti.is_set = true;
                    if (ti.clone_meth == null)
                    {
                        if (type.base_type is ICommonTypeNode)
                            ti.clone_meth = this.GetMethodBuilder(find_method(type.base_type as ICommonTypeNode, "CloneSet"));//ti.tp.GetMethod("Clone");
                        else
                            ti.clone_meth = ti.tp.GetMethod("CloneSet");
                    }
                    if (ti.assign_meth == null)
                    {
                        if (type.base_type is ICommonTypeNode)
                            ti.assign_meth = this.GetMethodBuilder(find_method(type.base_type as ICommonTypeNode, "AssignSetFrom"));
                        else
                            ti.assign_meth = ti.tp.GetMethod("AssignSetFrom");    
                    }
                    if (ti.def_cnstr == null)
                    {
                        if (type.base_type is ICommonTypeNode)
                            ti.def_cnstr = this.GetConstructorBuilder(find_constructor_with_params(type.base_type as ICommonTypeNode));
                        else
                            ti.def_cnstr = find_constructor_with_params(ti.tp);
                    }
                    return ti;
                case type_special_kind.diap_type:
                    return GetTypeReference(type.base_type);
                case type_special_kind.short_string:
                    return new TypeInfo(TypeFactory.StringType);
                case type_special_kind.array_kind:
                    TypeInfo tmp = GetTypeReference(type.element_type);
                    if (tmp == null) return null;
                    int rank = (type as ICommonTypeNode).rank;
                    if (rank == 1)
                    	ti = new TypeInfo(tmp.tp.MakeArrayType());
                    else
                    	ti = new TypeInfo(tmp.tp.MakeArrayType(rank));
                    //ti.is_arr = true;
                    defs[type] = ti;
                    return ti;
            }
			if (type is IRefTypeNode) {
				TypeInfo ref_ti = GetTypeReference(((IRefTypeNode)type).pointed_type);
                if (ref_ti == null) return null;
                //(ssyy) Better to use MakePointerType
                ti = new TypeInfo(ref_ti.tp.MakePointerType());
                defs[type] = ti;
                return ti;
			}
			
			return null;
		}
		
		public MethodBuilder GetMethodBuilder(IFunctionNode meth)
		{
			MethInfo mi = defs[meth] as MethInfo;
			if (mi != null)
			return mi.mi as MethodBuilder;
			return null;
		}
		
		public ConstructorBuilder GetConstructorBuilder(IFunctionNode meth)
		{
			MethInfo ci = defs[meth] as MethInfo;
			if (ci != null)
			return ci.cnstr as ConstructorBuilder;
			return null;
		}
		
        //get the array creation method
		public MethodInfo GetArrayInstance()
		{
			if (arr_mi != null) return arr_mi;
			arr_mi = typeof(System.Array).GetMethod("CreateInstance",new Type[2]{typeof(System.Type),typeof(int)});
			return arr_mi;
		}

        //add a stub method (if a method contains nested methods, a stub is created)
        //i.e., the method is not added to the table
        public MethInfo AddFictiveMethod(IFunctionNode func, MethodBuilder mi)
        {
            MethInfo m = new MethInfo(mi);
            //defs[func] = m;
            return m;
        }
    }
}